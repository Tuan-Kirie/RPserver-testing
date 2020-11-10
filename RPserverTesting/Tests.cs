using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit;
using NUnit.Framework;


namespace RPserverTesting
{
    [TestFixture]
    public class TestConnection
    {
        private Adapter adapter;
        private Process _rpserver;
        
        [SetUp]
        public void Setup()
        {
             _rpserver = Process.Start("C:\\RPSoftware\\Applications\\Server\\RPserver.exe");
             adapter = new Adapter();
        }
        
        [Test]
        public void CheckConnectionWithServer()
        {
            try
            {
                adapter.ConnectTeacher();
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
                
            }
        }
    
        [Test]
        public void CheckConnectionFirstMessage()
        {
            adapter.ConnectTeacher();
            IdVObject obj = new IdVObject();
            obj.id = "RP5-00-(000)";
            obj.version = "5.6.2.83";
            adapter.SendMessageToServer(obj);
            JObject mes = adapter.RecieveMessage();
            Assert.AreEqual("0", mes?["exercise_id"]?.Value<string>());
            Assert.AreEqual("RP5-00-(000)", mes?["id"]?.Value<string>());
            Assert.AreEqual("5.6.2.6487.1119", mes?["version"]?.Value<string>());
        }

        
        //Просто для теста левый гуид кидаем, к сожаление ему пофиг =)
        [Test]
        public void CheckConnectionFirstMessageError()
        {
            adapter.ConnectTeacher();
            IdVObject obj = new IdVObject();
            obj.id = "NEgativeGUID";
            obj.version = "5.6.2.83";
            adapter.SendMessageToServer(obj);
            JObject mes = adapter.RecieveMessage();
            Assert.AreEqual("NEgativeGUID", mes?["id"]?.Value<string>());
        }

        [Test]
        public void CheckConnectionSecondMessage()
        {
            adapter.ConnectTeacher();
            IdVObject obj = new IdVObject();
            obj.id = "RP5-00-(000)";
            obj.version = "5.6.2.83";
            adapter.SendMessageToServer(obj);
            JObject mes = adapter.RecieveAwaytingMessage("client_conn", "0");
            Assert.AreEqual("RP5-00-(000)", mes?["id"]?.Value<string>());
        }
        
        [Test]
        public void CheckConnectionThirdMessage()
        {
            adapter.ConnectTeacher();
            IdVObject obj = new IdVObject();
            obj.id = "RP5-00-(000)";
            obj.version = "5.6.2.83";
            adapter.SendMessageToServer(obj);
            JObject mes = adapter.RecieveAwaytingMessage("client_disconn", "100");
            Assert.AreEqual("RP5-00-(000)", mes?["id"]?.Value<string>());
        }

        [Test]
        public void TestPingPong()
        {
            adapter.ConnectTeacher();
            IdVObject obj = new IdVObject();
            obj.id = "RP5-00-(000)";
            obj.version = "5.6.2.83";
            adapter.SendMessageToServer(obj);
            for (int i = 0; i < 10; i++)
            {
                while (true)
                {
                    JObject response = adapter.RecieveMessage();
                    if (response?["ping"]?.ToString() == "0")
                    {
                        adapter.checkPingPong(response);
                        break;
                    }
                }
            }
            Assert.True(true);
        }
        
        [TearDown]
        public void TearDown()
        {
            adapter.client.Close();
            _rpserver.Close();
        }
        
    }
}