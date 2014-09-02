using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;


namespace TCPWindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        /* NetworkStream that will be used */
        private static NetworkStream myStream;
        /* TcpClient that will connect for us */
        private static TcpClient myClient;
        /* Storage space */
        private static byte[] myBuffer;
        /* Application running flag */
        private static bool bActive = true;

        private static Int32 sent;
        private static Int32 good;
        private static Int32 Bad;
        private static byte[] mySessNo = new byte[4];

        Thread tidListen = null;
        Thread tidSend = null;


        public Form1()
        {
            InitializeComponent();

            /* Vital: Create listening thread and assign it to ListenThread() */
             tidListen = new Thread(new ThreadStart(ListenThread));
            /* Vital: Create sending thread and assign it to SendThread() */
            tidSend = new Thread(new ThreadStart(SendThread));


        }

        private void UpdateText(string text)
        {
            // Set the textbox text.
            richTextBox1.Text = text;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

      
           
            
        }


        /* Thread responsible for "remote input" */
        public void ListenThread()
        {
            //Console.WriteLine("Listening...");

            richTextBox1.Invoke(new EventHandler(delegate
            {
                richTextBox1.AppendText("Listening...");

            }));


            while (bActive)
            {
                /* Reading data from socket (stores the length of data) */
                int lData = myStream.Read(myBuffer, 0,
                      myClient.ReceiveBufferSize);
                 /* String conversion (to be displayed on console) */
                String myString = Encoding.ASCII.GetString(myBuffer);
    
                /* Trimming data to needed length, 
                   because TcpClient buffer is 8kb long */
                /* and we don't need that load of data 
                   to be displayed at all times */
                /* (this could be done better for sure) */
                myString = myString.Substring(0, lData);
                /* Display message   */
                //Console.Write(myString);

                richTextBox1.Invoke(new EventHandler(delegate
                {
                    richTextBox1.AppendText(myString);
                    richTextBox1.AppendText("\n");

                }));

               
               // check if it is a reg packet and then extract the session number
                //if (myBuffer[0] == 0x65)
                //{
                //    if (myBuffer[1] == 00)
                //    {
                //        mySessNo[0] = myBuffer[4];
                //        mySessNo[1] = myBuffer[5];
                //        mySessNo[2] = myBuffer[6];
                //        mySessNo[3] = myBuffer[7];
                //    }
                //}

                // check to see if it's data comming back.
                //if (myBuffer[0] == 0x6f)  // RR data
                //{
                //    bool success = false;
                //    bool lengthGood = false;
                //    bool dataGood = false;

                //    int length = (myBuffer[3] << 8) | (myBuffer[2]);
                //    if (length == 266) //  (lData - 24))
                //    {
                //        lengthGood = true;
                //    }
                //    else
                //    {
                //        lengthGood = false;
                //    }


                //    byte[] handle = new byte[4];
                //    handle[0] = myBuffer[4];
                //    handle[1] = myBuffer[5];
                //    handle[2] = myBuffer[6];
                //    handle[3] = myBuffer[7];


                //    if ((myBuffer[8] == 0x00) & (myBuffer[9] == 0x00) & (myBuffer[10] == 0x00) & (myBuffer[11] == 0x00))
                //    {
                //        success = true;
                //    }
                //    else
                //    {
                //        success = false;
                //    }

                //    byte[] myData = new byte[266];

                //    for (int i = 0; i <= 265; i++)
                //    { myData[i] = 0xaa; 
                //    }
                   

                //    byte[] correctData = {0x00,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x00,0x00,0xb2,0x00,0xfa,0x00,0x8e,
                //    0x00,0x00,0x00,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x0b,0xfe,0x00,0x00,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0x00,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0x00,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0x00,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0x00,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0x00,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,0xfe,0x00,
                //    0xfe,0x00,0xfe,0x00,0xfe,0x00,0x00,0x03,0x06,0x30,0x39,0x00,0x10,0x50,0x43,0x32,0x31,0x5f,0x46,0x45,
                //    0x20,0x54,0x65,0x73,0x74,0x00,0x00,0x00,0x00};

                //    Buffer.BlockCopy(myBuffer,24,myData,0,266);

                //    dataGood = ByteArrayCompare(myData, correctData);


                //    if (dataGood & lengthGood & success)
                //    {
                //        good += 1;
                //        //sendCIP();
                //        //textGood.Text = string.Format("{0}", good);
                //    }
                //    else
                //    {
                //        Bad += 1;

                //        if (dataGood == false)
                //        {
                //            richTextBox1.Invoke(new EventHandler(delegate
                //            {
                //                myString = BitConverter.ToString(myData);

                //                richTextBox1.AppendText(myString);
                //                richTextBox1.AppendText("\n");

                //            }));
                //        }

                //        if (lengthGood == false)
                //        {
                //            richTextBox1.Invoke(new EventHandler(delegate
                //            {
                //                myString = string.Format("{0}\n",length);

                                
                //                richTextBox1.AppendText("length error !\n");
                //                richTextBox1.AppendText(myString);

                //            }));
                //        }

                //        if (success == false)
                //        {
                //            richTextBox1.Invoke(new EventHandler(delegate
                //            {
                //               // myString = BitConverter.ToString(myData);


                //                richTextBox1.AppendText("success error !\n");

                //            }));
                //        }

                        

                //    }
                //    // check data packet
 




                //}

            }

        }

        static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }


        /* Thread responsible for "local input" */
        private void SendThread()
        {
            while (bActive)
            {
               // sendCIP();

                Console.WriteLine("Sending...");
                while (bActive)
                {
                    /* Simple prompt */
                    Console.Write("> ");
                    /* Reading message/command from console */
                    String myString = Console.ReadLine() + "\n";
                    /* Sending the data */
                    myStream.Write(Encoding.ASCII.GetBytes(
                         myString.ToCharArray()), 0, myString.Length);
                }



            }
            


//            //Console.WriteLine("Sending...");

//            richTextBox1.Invoke(new EventHandler(delegate
//            {
//                richTextBox1.AppendText("Sending...");

//            }));
            

//            while (bActive)
//            {
//                /* Simple prompt */
//                Console.Write("> ");
//                /* Reading message/command from console */
////                String myString = Console.ReadLine() + "\n";
//                //send CIP frame
//                //String myString = "test";
                
//                byte[] data2 = { 0x6f, 0x00, 0x18, 0x00, 0x00, 0x07, 0x02, 0x30, 0x00, 0x00, 
//                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
//                                 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xb2, 0x00, 0x08, 0x00, 0x0e, 0x03, 
//                                 0x20, 0x90, 0x24, 0x01, 0x30, 0x01 };
               
//                /* Sending the data */
//                //myStream.Write(Encoding.ASCII.GetBytes(myString.ToCharArray()), 0, myString.Length);
//                myStream.Write(data2, 0, data2.Length);




            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Console.Write("Enter server name/address: ");
            String strServer = textBox1.Text;  //Console.ReadLine();

            //Console.Write("Enter remote port: ");
            String strPort = textBox2.Text;  //Console.ReadLine();

            /* Connecting to server (will crash if address/name is incorrect) */

           // try
           // {
            myClient = new TcpClient(strServer, Int32.Parse(strPort));

            

            textBox3.Text = "connected"; //Console.WriteLine("Connected...");
            /* Store the NetworkStream */
            myStream = myClient.GetStream();
            /* Create data buffer */
            myBuffer = new byte[myClient.ReceiveBufferSize];

            
            ///* Vital: Create listening thread and assign it to ListenThread() */
            //Thread tidListen = new Thread(new ThreadStart(ListenThread));
            ///* Vital: Create sending thread and assign it to SendThread() */
            //Thread tidSend = new Thread(new ThreadStart(SendThread));

           // Console.WriteLine("Application connected and ready...");
           // Console.WriteLine("----------------------------------");

            /* Start listening thread */
            tidListen.Start();
            /* Start sending thread */
           // regSession();
            
           // tidSend.Start();

                
            //}
            //catch(System.Net.Sockets.SocketException se)
            //{
             //   richTextBox1.AppendText(se.Message);
              //  richTextBox1.AppendText("\n");
            //}
            
            
          
        }



        private void button2_Click(object sender, EventArgs e)
        {
            //sendCIP();
            good = 0;
            sent = 0;
            Bad = 0;

            timer1.Enabled = true;
        }

        public void sendCIP()
        {
            //Console.WriteLine("Sending...");

            //richTextBox1.Invoke(new EventHandler(delegate
            //{
            //    richTextBox1.AppendText("Sending...");

            //}));



            /* Simple prompt */
            //Console.Write("> ");
            /* Reading message/command from console */
            //                String myString = Console.ReadLine() + "\n";
            //send CIP frame
            //String myString = "test";

            //byte[] data2 = { 0x6f, 0x00, 0x18, 0x00, 0x00, 0x07, 0x02, 0x30, 0x00, 0x00, 
            //                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            //                 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xb2, 0x00, 0x08, 0x00, 0x0e, 0x03, 
            //                 0x20, 0x90, 0x24, 0x01, 0x30, 0x01 };

            byte[] data2 = { 0x6f, 0x00, 0x18, 0x00, mySessNo[0], mySessNo[1], mySessNo[2], mySessNo[3], 0x00, 0x00, 
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xb2, 0x00, 0x08, 0x00, 0x0e, 0x03, 
                                 0x20, 0x90, 0x24, 0x01, 0x30, 0x01 };


            /* Sending the data */
            //myStream.Write(Encoding.ASCII.GetBytes(myString.ToCharArray()), 0, myString.Length);
            myStream.Write(data2, 0, data2.Length);
            sent += 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            regSession();
        }

        private void regSession()
        {
            //Console.WriteLine("Sending...");

            richTextBox1.Invoke(new EventHandler(delegate
            {
                richTextBox1.AppendText("Sending...");

            }));



            /* Simple prompt */
            Console.Write("> ");
            /* Reading message/command from console */
            //                String myString = Console.ReadLine() + "\n";
            //send CIP frame
            //String myString = "test";

            byte[] data2 = { 0x65, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 
                                 0x00, 0x00 };

            /* Sending the data */
            //myStream.Write(Encoding.ASCII.GetBytes(myString.ToCharArray()), 0, myString.Length);
            myStream.Write(data2, 0, data2.Length);
        }

        private void getFunctions()
        {
            //Console.WriteLine("Sending...");

            richTextBox1.Invoke(new EventHandler(delegate
            {
                richTextBox1.AppendText("get..\n");

            }));



            /* Simple prompt */
            Console.Write("> ");
            /* Reading message/command from console */
                            String myString = Console.ReadLine() + "\n";
            //send CIP frame
            //String myString = "test";

                            byte[] data2 = { 0x10, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x00, 0x00 };

            /* Sending the data */
            //myStream.Write(Encoding.ASCII.GetBytes(myString.ToCharArray()), 0, myString.Length);
            myStream.Write(data2, 0, data2.Length);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("Sending...");

            richTextBox1.Invoke(new EventHandler(delegate
            {
                richTextBox1.AppendText("Sending...");

            }));



            /* Simple prompt */
            Console.Write("> ");
            /* Reading message/command from console */
            //                String myString = Console.ReadLine() + "\n";
            //send CIP frame
            //String myString = "test";

            byte[] data2 = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            /* Sending the data */
            //myStream.Write(Encoding.ASCII.GetBytes(myString.ToCharArray()), 0, myString.Length);
            myStream.Write(data2, 0, data2.Length);


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sendCIP();
            //inc sent count
            //sent += 1;
            textSent.Text = string.Format("{0}", sent);
            textGood.Text = string.Format("{0}", (good + 1));
            textBad.Text = string.Format("{0}", Bad);
        }

        private void textSent_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            myStream.Close();
            myClient.Close();
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            getFunctions();
        }
    }
}
