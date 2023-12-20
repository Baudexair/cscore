using System;
using System.Collections.Generic;
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
        public List<Message> convlist
        public string getName(){
            return name;
        }
        public void receiveMessage(string pMessage, Agent pTransmitter){
            this.convlist.add(new Message(pTransmitter, this, pMessage));
        }
        public void sendMessage(string pMessage, Agent pReceiver){
          receiver.receiveMessage(pMessage, this);
          this.convlist.add(new Message(this, pReceiver, pMessage));
        }
        public void resetConvo(){
            this.convlist = new List<Message>();
        }
        private void resetConvo(Agent pAgent){
            foreach (m in this.convlist){
                if(isEqual(m.receiver,pAgent)||isEqual(m.transmitter,pAgent)){
                    convlist.remove(m);
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

        public var generate(string pmessage){
            var openAi = new OpenAi(await IoC.inject.GetAppSecrets().GetSecret(aiKey));
            var messages = new List<ChatGpt.Line>();
            messages.Add(new ChatGpt.Line(ChatGpt.Role.system, content: role));

            messages.Add(pmessage);

                
            // Send the messages to the AI and get the response:
            var result = await openAi.Complete(messages);
            var completion = result.choices.Single().text;
            if (Assert.NotEmpty(completion)){
                return completion;
            }
            return null;
        }

    }