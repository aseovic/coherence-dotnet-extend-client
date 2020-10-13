/*
 * Copyright (c) 2000, 2020, Oracle and/or its affiliates.
 *
 * Licensed under the Universal Permissive License v 1.0 as shown at
 * http://oss.oracle.com/licenses/upl.
 */
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Tangosol.Run.Xml;

namespace Tangosol.Net.Ssl
{
    [TestFixture]
    public class SslTest 
    {
        private const string serverCert = "./Net/Ssl/Server.cer";
        private const string clientCert = "./Net/Ssl/Client.pfx";
        private const string clientCertPassword = "password";

        private SslServer server;

        [SetUp]
        public void SetUp()
        {
            server = null;
        }

        [TearDown]
        public void TearDown()
        {
            server = null;
        }


        [Test]
        public void TestSslServerAuthentication()
        {
            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            Console.WriteLine(Directory.GetCurrentDirectory());
            server = new SslServer(location)
            {
                ServerCertificate =
                        SslServer.LoadCertificate(serverCert)
            };
            server.Start();

            SslClient client =
                new SslClient(location)
                    {
                        ServerName = "MyServerName",
                        Protocol   = SslProtocols.Default
                    };
            try
            {
                client.Connect();

                string echo = client.Echo("Hello World");
                Assert.AreEqual(echo, "Hello World");
            }
            finally
            {
                client.Close();
                server.Stop();
                client = null;
            }
        }

        [Test]
        public void TestSslClientAuthenticationException()
        {
            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            server = new SslServer(location)
                    {
                        ServerCertificate  = SslServer.LoadCertificate(serverCert),
                        AuthenticateClient = true
                    };
            server.Start();

            SslClient client =
                    new SslClient(location)
                        {
                            ServerName   = "MyServerName",
                            Protocol     = SslProtocols.Default,
                            Certificates = null
                        };
            try
            {
                Assert.That(() => client.Connect(), Throws.TypeOf<IOException>());
            }
            finally
            {
                client.Close();
                server.Stop();
            }
        }

        [Test]
        public void TestSslClientAuthentication()
        {
            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            server = new SslServer(location)
            {
                ServerCertificate = SslServer.LoadCertificate(serverCert),
                AuthenticateClient = true
            };
            server.Start();

            SslClient client =
                    new SslClient(location)
                        {
                            ServerName = "MyServerName",
                            Protocol   = SslProtocols.Default
                        };
            try
            {
                client.AppendCertificate(new X509Certificate(clientCert, clientCertPassword));
                client.Connect();

                string echo = client.Echo("Hello World");
                Assert.AreEqual(echo, "Hello World");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                client.ClearCertificate();
                client.Close();
                server.Stop();
            }
        }

        [Test]
        public void TestSslClientConfiguration()
        {
            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            server = new SslServer(location)
            {
                ServerCertificate =
                        SslServer.LoadCertificate(
                        serverCert),
                AuthenticateClient = true
            };
            server.Start();
            TcpClient client = new TcpClient();
            try
            {
                IXmlDocument xmlDoc = XmlHelper.LoadXml("./Net/Ssl/Configs/config.xml");
                
                IStreamProvider streamProvider = StreamProviderFactory.CreateProvider(xmlDoc);

                client.Connect(location);
                Stream stream = streamProvider.GetStream(client);
                
                string echo = SslClient.Echo(stream, "Hello World");
                Assert.AreEqual(echo, "Hello World");
            }
            finally
            {
                client.Close();
                server.Stop();
                client = null;
            }
        }

        [Test]
        public void TestSslClientConfiguration2()
        {
            IXmlDocument xmlDoc = XmlHelper.LoadXml("./Net/Ssl/Configs/config2.xml");

            Assert.That(() => StreamProviderFactory.CreateProvider(xmlDoc), Throws.TypeOf<TypeLoadException>());
        }

        [Test]
        public void TestSslClientConfiguration3()
        {
            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            server = new SslServer(location)
            {
                ServerCertificate =
                        SslServer.LoadCertificate(
                        serverCert),
                AuthenticateClient = true
            };
            server.Start();
            TcpClient client = new TcpClient();
            try
            {
                IXmlDocument xmlDoc = XmlHelper.LoadXml("./Net/Ssl/Configs/config3.xml");

                IStreamProvider streamProvider = StreamProviderFactory.CreateProvider(xmlDoc);

                client.Connect(location);
                Stream stream = streamProvider.GetStream(client);

                string echo = SslClient.Echo(stream, "Hello World");
                Assert.AreEqual(echo, "Hello World");
            }
            finally
            {
                client.Close();
                server.Stop();
                client = null;
            }
        }

        [Test]
        //[ExpectedException(typeof(AuthenticationException))]
        public void TestSslClientConfiguration4()
        {
            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            server = new SslServer(location)
            {
                ServerCertificate =
                        SslServer.LoadCertificate(
                        serverCert),
                AuthenticateClient = true
            };
            server.Start();
            TcpClient client = new TcpClient();
            try
            {
                IXmlDocument xmlDoc = XmlHelper.LoadXml("./Net/Ssl/Configs/config4.xml");

                IStreamProvider streamProvider = StreamProviderFactory.CreateProvider(xmlDoc);

                client.Connect(location);
                Stream stream = streamProvider.GetStream(client);

                string echo = SslClient.Echo(stream, "Hello World");
                Assert.AreEqual(echo, "Hello World");
            }
            finally
            {
                client.Close();
                server.Stop();
                client = null;
            }
        }

        [Test]
        public void TestSslClientConfiguration5()
        {
            IXmlDocument xmlDoc = XmlHelper.LoadXml("./Net/Ssl/Configs/config5.xml");
            Assert.That(() => StreamProviderFactory.CreateProvider(xmlDoc), Throws.InstanceOf<CryptographicException>());
        }

        [Test]
        public void TestSslConfigurationWithSelector()
        {
            IXmlDocument xmlDoc = XmlHelper.LoadXml("./Net/Ssl/Configs/config6.xml");
            Assert.NotNull(xmlDoc);

            IStreamProvider streamProvider = StreamProviderFactory.CreateProvider(xmlDoc);
            Assert.NotNull(streamProvider);
            Assert.IsTrue(streamProvider is SslStreamProvider);

            SslStreamProvider sslStreamProvider = (streamProvider as SslStreamProvider);
            Assert.IsTrue(sslStreamProvider.LocalCertificateSelector is LocalCertificateSelectionCallback);

            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            server = new SslServer(location)
            {
                ServerCertificate = SslServer.LoadCertificate(serverCert),
                AuthenticateClient = true
            };
            server.Start();

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(location);
                Stream stream = streamProvider.GetStream(client);

                string echo = SslClient.Echo(stream, "Hello World");
                Assert.Fail("Expected Exception, but got none");
            }
            catch (Exception e)
            {
                Console.WriteLine("SslTests.TestSslConfigurationWithSelector(), exception: " + e.ToString());
                Assert.IsTrue(e is AuthenticationException);
                Assert.NotNull(e.Message);
                Assert.IsTrue(e.Message.Contains("RemoteCertificateNameMismatch"));
            }
            finally
            {
                client.Close();
                server.Stop();
                client = null;
            }
        }

        [Test]
        public void TestSslRemoteCertValidation()
        {
            IXmlDocument xmlDoc = XmlHelper.LoadXml("./Net/Ssl/Configs/config7.xml");
            Assert.NotNull(xmlDoc);

            IStreamProvider streamProvider = StreamProviderFactory.CreateProvider(xmlDoc);
            Assert.NotNull(streamProvider);
            Assert.IsTrue(streamProvider is SslStreamProvider);

            SslStreamProvider sslStreamProvider = (streamProvider as SslStreamProvider);
            Assert.IsTrue(sslStreamProvider.RemoteCertificateValidator is RemoteCertificateValidationCallback);

            var location = new IPEndPoint(IPAddress.Loopback, 5055);
            server = new SslServer(location)
            {
                ServerCertificate = SslServer.LoadCertificate(serverCert),
                AuthenticateClient = true
            };
            server.Start();

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(location);
                Stream stream = streamProvider.GetStream(client);
                Assert.NotNull(stream);
                Assert.AreEqual(sslStreamProvider.ServerName, "MyServerName");

                string echo = SslClient.Echo(stream, "Hello World");
                Assert.AreEqual(echo, "Hello World");
            }
            finally
            {
                client.Close();
                server.Stop();
                client = null;
            }
        }
    }
}
