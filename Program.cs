using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using AsterNET.Manager;
using AsterNET.Manager.Action;
using AsterNET.Manager.Response;

namespace CheckBalance
{
    class Program
    {
        public static Dictionary<string, CallInfo> channelWiseCallInfo= new Dictionary<string,CallInfo>();
        private static String TestChannelId;
        
        static void Main(string[] args)
        {
            
            
            //Create Client Conenction (Asterisk AMI Integration)
            ManagerConnection managerConnection =
                new ManagerConnection("192.168.0.125", 5038, "Asterisk-Easin", "121212");
            
            try
            {
                managerConnection.Login(); //AMI Login
                
                //Custom AMI command to get User Information
                string customCommandAction = "Originate 192.168.0.125/121212 extension 323232@register";
                CommandAction commandAction = new CommandAction(customCommandAction);
                
                // Send the action and wait for a response
                int timeoutMilliseconds = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                ManagerResponse response = managerConnection.SendAction(commandAction, timeoutMilliseconds);
                
                // Register for the events you're interested in
                managerConnection.NewChannel += (sender, e) =>
                {
                    if (e.UniqueId == e.Attributes.ElementAt(4).Value)
                    {
                        TestChannelId = e.Channel;
                        // Console.WriteLine(TestChannelId);
                        CallInfo callInfo = new CallInfo(e.CallerIdNum,e.Channel);
                        channelWiseCallInfo.Add(callInfo.ChannelId,callInfo); 
                    }
 
                };
                
                //Step 2: After Registration Is Successfull
                
                managerConnection.NewState += (sender, e) =>
                {

                    // Console.WriteLine(e);

                    for (int i = 0; i < channelWiseCallInfo.Count; i++)
                    {
                        // Console.WriteLine(channelWiseCallInfo.ElementAt(i).Value.ChannelId);
                    }

                    if (e.Channel == TestChannelId)
                    {
                        CallInfo callInfo = channelWiseCallInfo[TestChannelId];
                        callInfo.AnswerInfo = e;
                        Console.WriteLine(callInfo.AnswerInfo);
                    }

                    // Console.WriteLine($"{callInfo.ChannelId}-----{callInfo.CalledId}-------{channelWiseCallInfo.Count}");
                    // string channel = e.Channel;
                    //     
                    // callInfo.CalledId = e.Connectedlinenum;
                    // callInfo.InviteInfo = e;

                };
                //Step 3: Details After Call Is Hungup
                managerConnection.Hangup += (sender, e) =>
                {
               
                   
                };
                
                // Wait for user input to keep the program running ***
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                // Close the connection when done
                managerConnection.Logoff();
            }
        }
    }
}