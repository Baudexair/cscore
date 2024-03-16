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
        public List<Agent> receiver;
        public string message;
        public Message(Agent pTransmitter, List<Agent> pReceiver, string pMessage){
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
            List<Agent> list = new List<Agent>();
            list.Add(this);
            this.convlist.Add(new Message(pTransmitter, list, pMessage));
        }
        public void sendMessage(string pMessage, Agent pReceiver){
          pReceiver.receiveMessage(pMessage, this);
          List<Agent> list = new List<Agent>();
          list.Add(pReceiver);
          this.convlist.Add(new Message(this, list, pMessage));
        }

        public void sendMessage(string pMessage,List<Agent> agents){
            foreach (Agent a in agents)
            {
                a.receiveMessage(pMessage, this);
            }
            this.convlist.Add(new Message(this, agents, pMessage));

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
        
        public async Task<string> generateAsync(string pmessage)
        {
            var openAi = new OpenAi(aiKey);
            var messages = new List<ChatGpt.Line>() {
                new ChatGpt.Line(ChatGpt.Role.system, content: role),
                new ChatGpt.Line(ChatGpt.Role.user, content: pmessage),
            };
            var request = new ChatGpt.Request(messages);
            request.model = "gpt-3.5-turbo-1106"; // See https://platform.openai.com/docs/models/gpt-4
            var response = await openAi.ChatGpt(request);
            ChatGpt.Line newLine = response.choices.Single().message;
            messages.Add(newLine);
            return "response.content=" + JsonWriter.AsPrettyString(messages);

        }
        public async Task<string> generateAsyncAnswerOnly(string pmessage)
        {
            var openAi = new OpenAi(aiKey);
            var messages = new List<ChatGpt.Line>() {
                new ChatGpt.Line(ChatGpt.Role.system, content: role),
                new ChatGpt.Line(ChatGpt.Role.user, content: pmessage),
            };
            var request = new ChatGpt.Request(messages);
            request.model = "gpt-3.5-turbo-1106"; // See https://platform.openai.com/docs/models/gpt-4
            var response = await openAi.ChatGpt(request);
            ChatGpt.Line newLine = response.choices.Single().message;
            return  JsonWriter.AsPrettyString(newLine);
        }
         public async Task<string> generateAsyncAnswerOnlyJson(string pmessage, string character)
        {
            var openAi = new OpenAi(aiKey);
            var messages = new List<ChatGpt.Line>();
            messages.Add(new ChatGpt.Line(ChatGpt.Role.system, content: "You are a helpful assistant designed to output JSON about your character:"+ character));

            { // The user inputs a question but the response should be automatically parsable as a YesNoResponse:

                // Create an example object so that the AI knows how the response json should look like for user inputs:
                var yesNoResponseFormat = new YesNoResponse() {
                    confidence = 100,
                    inputQuestionInterpreted = "Is your character fictional?",
                    yesNoAnswer = true,
                    explanation = "My character Iron Man is a fictional superhero from Marvel."
                };
                messages.AddUserLineWithJsonResultStructure(pmessage, yesNoResponseFormat);

                // Send the messages to the AI and get the response:
                var request = new ChatGpt.Request(messages);
                request.model = "gpt-3.5-turbo-1106"; // See https://platform.openai.com/docs/models/gpt-4
                var response = await openAi.ChatGpt(request);
                ChatGpt.Line newLine = response.choices.Single().message;
                messages.Add(newLine);

                // Parse newLine.content as a YesNoResponse:
                var yesNoResponse = newLine.ParseNewLineContentAsJson<YesNoResponse>();

                if(yesNoResponse.yesNoAnswer){return "Yes";} else {return "No";}
                
        }

    }
    public class YesNoResponse {

            [Description("The confidence of the AI in the answer")]
            public int confidence { get; set; }

            [Description("The summary of the input question that the AI used to give the answer")]
            public string inputQuestionInterpreted { get; set; }

            [Description("The yes/no decision of the AI for the input question")]
            public bool yesNoAnswer { get; set; }

            [Description("The explanation of the AI why it gave the answer")]
            public string explanation { get; set; }

        }
    }
}