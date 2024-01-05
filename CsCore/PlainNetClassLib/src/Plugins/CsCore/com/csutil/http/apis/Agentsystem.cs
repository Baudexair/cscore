using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.csutil.model.jsonschema;
using Newtonsoft.Json;


namespace com.csutil.http.apis {
    public class Agentsystem{
        public List<Agent> agents;
        public string aiKey;



    }
    public class Message{
        public Agent transmitter;
        public Agent receiver;
        public string message;
        public Message(Agent pTransmitter, Agent pReceiver, string pMessage){
            transmitter=pTransmitter;
            receiver=pReceiver;
            message=pMessage;
        }
    }
    public class Agent{
        public Agent(string pName, string pRole, string pKey){
            name=pName;
            notes="";
            role=pRole;
            aiKey=pKey;
            convlist = new List<Message>();
        }
        public string name;
        public string notes;
        public string role;
        public string aiKey;
        public List<Message> convlist;
        public string getName(){
            return name;
        }
        public void receiveMessage(string pMessage, Agent pTransmitter){
            this.convlist.Add(new Message(pTransmitter, this, pMessage));
        }
        public void sendMessage(string pMessage, Agent pReceiver){
          pReceiver.receiveMessage(pMessage, this);
          this.convlist.Add(new Message(this, pReceiver, pMessage));
        }
        public void resetConvo(){
            this.convlist = new List<Message>();
        }
        private void resetConvo(Agent pAgent){
            foreach (Message m in this.convlist){
                if(Equals(m.receiver,pAgent)||Equals(m.transmitter,pAgent)){
                    convlist.Remove(m);
                }
            }
        }
        public void resetConvos(Agent pAgent){
            this.resetConvo(pAgent);
            pAgent.resetConvo(this);
        }
        public string getNotes(){
            return notes;
        }
        public void setNotes(string pNotes){
            notes= pNotes;
        }
        //Wir müssem das generate nochmal komplett überarbeiten... So funktioniert das aktuell nicht.
        public async Task<string> generateAsync(string pmessage)
        {
            var openAi = new OpenAi(aiKey);
            var prompt = "Your role is: "+ role + " your prompt: " + pmessage;
            var result = await openAi.Complete(prompt);
            var completion = result.choices.Single().text;
            return "prompt:" +pmessage+ "answer" +completion;

        }

    }
}