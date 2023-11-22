internal class Program
{
    private static void Main(string[] args)
    {
        //işletim sistemi sistemin kitlenmesine veyahut yanlış çalışmasına sebep olacak durumlar
        //her thread main threadin yanında çalışır ve belli bir süre sonra bitmek zorundadır
        //buna 20ms desek max çalışma süresidir ve bu zamanlma Round Robindir
        //main thread her zaman daha hızlıdır 

        /*
        Console.WriteLine("Threadler başladı");
        Thread th1 = new Thread(arttıra);
        Thread th2 =new Thread(arttıra);
        th1.Start();
        th2.Start();
        Console.WriteLine("Main bitti");
        th1.Join();
        th2.Join();
        Console.WriteLine(a); 

 */
        //YUKARIDA YAPILAN İŞLEM RACE CONDİTİON dur alttaki metod iki thread tarafından kullanıldığı için
        //burada yapılan şey kendilerinin kaç kere arttırdığının sayamamaları
        //FALSE SHİRİNG de corealrın cachelerinin sürekli veri çekmesi yüzündendir
        //kaç tane core varsa o kadar performanı düşürüz

        /*
        Thread th1 = new Thread(düzgünArtA);
        Thread th2 = new Thread(düzgünArtA);
        th1.Start();
        th2.Start();
        Console.WriteLine("Main bitti");
        th1.Join();
        th2.Join();
        Console.WriteLine(a); */
        //DÜZGÜN ÇIKTI ALDIK VE BURADA LOCK METODU KULLANILDI İÇ OBJE BOŞ GEÇEMEZ DİKKAT! CPUYU KİTLEDİ

        //DİĞER YÖNTEM
        /* Thread th1 = new Thread(enDüzgünArttır);
          Thread th2 = new Thread(enDüzgünArttır);
          th1.Start();
          th2.Start();
          Console.WriteLine("Main bitti");
          th1.Join();
          th2.Join();
          Console.WriteLine(a); */

        //BU BİRAZ DAHA PERFORMANSLI VE BUNA İNTERLOCKED CLASS DİYORUZ NOT: DONANIMI KULLANAN BİR CLASS
        /* Thread th1 = new Thread(arttırVeCheckh);
        Thread th2 = new Thread(arttırVeCheckh);
        th1.Start();
        th2.Start();
        Console.WriteLine("Main bitti");
        th1.Join();
        th2.Join();
        Console.WriteLine(a);*/ //birdiğer yöntem


        //EN AZ İKİ ADET SENKRONİZASYON ÖĞESİ VE THREADS İLE DEADLOCK OLABİLİR KİLİT AÇILMAZ

        /*Thread th1 = new Thread(DeadlockA);
        Thread th2 = new Thread(DeadlockB);
        th1.Start();
        th2.Start();
        Console.WriteLine("Main bitti"); //biri çalışırken eğer diğerinde obje kitli ise açılmasını bekliyor yani
        //bakış açısıyla bakarsan ikişer kilit not: çalıştırma pc çöp oluyor
        //   
        //    kitledi obj     kitledi obj2    bu yüzden ikiside diğerini bekliyor yani deadlock
        //
        //    bekliyor obj2   bekliyor obj
        */

        //SEMAPHONE
        //check metodunun 5 li semaphonelu hali sorulabilir 

        /*
        Console.WriteLine("Threadler başladı");
        for (int i = 0; i < 9; i++)
        {
            new Thread(semaphonearttır).Start(i);

        }
        sem.Release(); //BU YAZILMAZSA PATLAR DEADLOCK
        Console.WriteLine("main");
        */


        for (int i = 0; i < 100; i++)
        {
            Thread thread = new Thread(Uploudİmage);
            thread.Name = "THREAD :" + i;
            thread.Start(); //thread başladığı gibi zaten waitone ve release komutları bizim metodumuzun içindedi
        }
        
    }

    //EN ÖNEMLİ 
    static int s = 10; //sempahone kaç değer verilecek

    static void WaitOne()
    {
        while (s <= 0) ; //S 10 u geçmeden asla çalışmayacak
        Interlocked.Add(ref s,-1); //Thread girdi ve dedi ki birader senin 10 tane yerin var 
        //gir bekle eğer 10 kişi bekliyorsa napıcaksın bekliyceksin o sırada boşta olan S sayısı azaltıcaksın
    }

    static void Release() { 
    Interlocked.Add(ref s,+1); //İmplementasyon kısmı yukarıdaki 2 metod kadardır EFEM SAĞOLSUN
    }

    static void Uploudİmage()
    {
        Console.WriteLine("{0} wants to enter:",Thread.CurrentThread.Name);
        WaitOne();
        Console.WriteLine("{0} is entered",Thread.CurrentThread.Name);
        Thread.Sleep(5000);
        Release(); //5 saniye thread çalışma süresi diyelim
        Console.WriteLine("{0} is released:",Thread.CurrentThread.Name);

    }


    static int a = 0;
    static object obj = "";
    static object obj2 = "";
    static Semaphore sem = new Semaphore(0, 5);

    static int lockobject = 0;
    public static void arttıra()
    {
            Console.WriteLine("A artıyor");
        for (int i = 0; i < 1000000; i++)
            a++;
    }

    public static void semaphonearttır(object i)
    {
        sem.WaitOne();
        Console.WriteLine((int) i);
        Console.WriteLine("slm kritik alan");
        sem.Release();
    }

    public static void düzgünArtA()
    {
        
        Console.WriteLine("A artıyor");
        for (int i = 0; i < 1000000; i++)
        {
            lock (obj) //obje boş olamaz
                a++;

        }
    }

    public static void enDüzgünArttır()
    {
        
        Console.WriteLine("interlocked çalıştı");
        for (int i = 0; i < 1000000; i++)
        {
            Interlocked.Increment(ref a);
        }
    }
    public static void arttırVeCheckh()
    {
        Console.WriteLine("A artıyor");
        for (int i = 0; i < 1000000; i++)
        {
            Checkh();
            a++;
            Interlocked.Exchange(ref lockobject, 0);
        } //ESKİ SINAV SORULARINDAN
            
    }

    public static void Checkh()
    {
        while (Interlocked.Exchange(ref lockobject, 1) == 1) ; //exchange de ise değer sağdaki olur
        //fakat return olarak eski değeri döndürür yani burda 1 olmadan bu metod geri dönmeyecek
    }

    public static void DeadlockA()
    {

       
        lock (obj) //obje boş olamaz
            {
            Thread.Sleep(1000);
            lock (obj2)
                {
                Console.WriteLine("1. bitti");
            }
         }

        
    }
    public static void DeadlockB()
    {
       
        lock (obj2) //obje boş olamaz
         {
            Thread.Sleep(1000);
            lock (obj)
                {
                Console.WriteLine("2. bitti");
            }
         }

        
    }
}