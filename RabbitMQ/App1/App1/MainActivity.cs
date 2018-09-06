using Android.App;
using Android.Widget;
using Android.OS;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using Android.Views;
using System;
using Android.Support.V4.App;

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            Button btnSend = FindViewById<Button>(Resource.Id.Send);

            btnSend.SetOnClickListener(new SendListner(this));

            Button btnReceive = FindViewById<Button>(Resource.Id.Receive);

            btnReceive.SetOnClickListener(new ReceiverListner(this));
            ReceiveMessage();
        }


        public void SendMessage()
        {
            var factory = new ConnectionFactory() { HostName = "192.168.42.15", UserName = "developer", Password = "developer" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "private",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: true,
                                 arguments: null);

                    string message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "private",
                                         basicProperties: null,
                                         body: body);
                }
            }
        }

        public void ReceiveMessage()
        {
            var factory = new ConnectionFactory() { HostName = "192.168.42.15", UserName = "guest", Password = "guest" };
            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
                

                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this);
                        notificationBuilder.SetSmallIcon(Resource.Drawable.Icon);
                        notificationBuilder.SetContentTitle("Title");
                        notificationBuilder.SetContentText(message);
                        notificationBuilder.SetAutoCancel(true);

                        // Sets an ID for the notification
                        int notificationId = new Random().Next();
                        // Gets an instance of the NotificationManager service
                        NotificationManager notificationManager = (NotificationManager)this.GetSystemService(Android.Content.Context.NotificationService);

                        notificationManager.Notify(notificationId, notificationBuilder.Build());

                        //Console.WriteLine(" [x] Received {0}", message);
                    };
                    channel.BasicConsume(queue: "hello",
                                         autoAck: true,
                                         consumer: consumer);

                    //Console.WriteLine(" Press [enter] to exit.");
                    //Console.ReadLine();
                
            
        }

        public class SendListner : Java.Lang.Object, View.IOnClickListener
        {
            MainActivity activity;
            public SendListner(MainActivity activity)
            {
                this.activity= activity;
            }
            public void OnClick(View v)
            {
                this.activity.SendMessage();
            }
        }


        public class ReceiverListner : Java.Lang.Object, View.IOnClickListener
        {
            MainActivity activity;
            public ReceiverListner(MainActivity activity)
            {
                this.activity = activity;
            }
            public void OnClick(View v)
            {
                this.activity.ReceiveMessage();
            }
        }
    }
}

