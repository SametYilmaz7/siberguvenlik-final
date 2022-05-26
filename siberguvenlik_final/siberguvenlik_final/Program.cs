using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace siberguvenlik_vize
{
    class Program
    {
        static void Main(string[] args)
        {
            //nmap komutu için ihtiyacımız olan parametreleri oluşturuyoruz.
            string address;
            int port;

            while (true) //konsolda cikis yazılmadığı sürece program devam edecek. 
            {
                //Tarama işleminde kullanılacak alanlar için kullanıcıdan parametre alıyoruz

                Console.WriteLine("Cikmak icin 'cikis' yaziniz.");

                Console.Write("İnternet Adresini Giriniz: ");
                address = Console.ReadLine();

                if (address == "cikis") break; //cikis yazılırsa program sonlanıyor.

                Console.Write("Adresin portunu giriniz: ");
                port = Convert.ToInt32(Console.ReadLine());

                //Burada bir process oluşturuyoruz ve nmap.exe uygulamasını çalıştırıyoruz girilecek komutları "Arguments" kısmında belirtiyoruz ve processi başlatıyoruz.
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "nmap.exe";
                startInfo.Arguments = "-p " + port + " --script http-sql-injection " + address + " -oX \"c:/temp/cikti.xml\""; //Kullanıcının girdiği parametreleri burada kullanıyoruz. Bu komut sayesinde uygulamada yazılması gereken komutları yazıyoruz.

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();//Process işlemi bitene kadar herhangi bir kod çalışmasın diye bu fonksiyonu kullanıyoruz.

                Console.WriteLine("*************************Tarama islemi tamamlandi.*************************");

                //process de oluşturduğumuz xml dosyasını okutuyoruz ve ilgili "script" alanını alıyoruz.
                XmlDocument doc = new XmlDocument();
                doc.Load("c:/temp/cikti.xml");
                XmlNodeList elemList = doc.GetElementsByTagName("script");

                JArray result = new JArray(   //Nmap'te kullanılan komutu ve script alanındaki verileri bir json array oluşturup içine kaydediyoruz.
                   new JObject(
                            new JProperty("Nmap command", startInfo.Arguments)), //nmapteki komutu arguments kısmında kullandığım için o kısmı çağırıyorum.
                    new JObject(
                            new JProperty("Url", elemList[0].Attributes["output"].Value))); //Script alanından sadece output kısmını çağırıyorum

                // Bu kısımda json array'e kaydettigim veriler ayarlanan dosya yoluna json dosyası olarak oluşturuluyor.

                File.WriteAllText(@"C:\Users\samet\OneDrive\Masaüstü\deneme.json", result.ToString()); //Dosya yolu kullanılan bilgisayara göre ayarlanmalıdır. 
                using (StreamWriter dosya = File.CreateText(@"C:\Users\samet\OneDrive\Masaüstü\deneme.json"))
                using (JsonTextWriter yazdir = new JsonTextWriter(dosya))
                {
                    result.WriteTo(yazdir);
                }
                Console.WriteLine("*************************Json Dosyasi Olusturuldu*************************");

            }
        }






    }
}

