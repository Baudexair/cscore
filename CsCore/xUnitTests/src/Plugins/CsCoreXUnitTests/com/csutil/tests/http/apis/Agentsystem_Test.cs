using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using com.csutil.http.apis;
using com.csutil.model.jsonschema;
using Newtonsoft.Json.Linq;
using Xunit;

namespace com.csutil.integrationTests.http {
    public class OpenAiAgentTests{

        [Fact]
        public async Task Test1_Agentreview()
        {
            string key="";
            Agent agent1 = new Agent("Agent1", "Give an answer to the given prompt: ", key);
            Agent agent2 = new Agent("Agent2", "You will be given a prompt and an answer to this prompt. Verify if the answer is correct and grade it on a scale from 1 to 10, with 10 being the best: ", key);
            string answer = await agent1.generateAsync("When did WW2 end?");
            agent1.sendMessage(answer, agent2);
            string re = await agent2.generateAsync(agent2.convlist.Last().message);
            agent2.sendMessage(re,agent1);
            Assert.NotEmpty(re);
            Trace.WriteLine(answer +" Review: "+re);
            Console.WriteLine(answer +" Review: "+re);
            Log.d(answer + " Review:" + re);
        }

        [Fact]
        public async Task Test3_Agentguesser()
        {
            //agent 1 needs to answer only with one of the following answers: YES, NO, GAME OVER | It still does not work. Often agent1 spoilers by accident their identity, example: Is your character male? Yes Iron Man is male.
            string key="";
            Agent agent1 = new Agent("Agent1", "You are the given Character. Answer the following question about yourself with one of these answers \"YES\"|\"NO\". Write nothing else. If your identity is guessed, write \"GAME OVER\" and nothing else.", key);
            Agent agent2 = new Agent("Agent2", "You will be playing a guessing game. Ask questions until you correctly guess the identity of the other person. Try to keep your questions general until you are quite sure. Your previous questions and their answers will be given to you: ", key);
            string answer = await agent1.generateAsyncAnswerOnly("Choose a character for the guessing game and write only the character as Output.");
            agent1.setNotes(answer);
            Trace.WriteLine("Character: "+answer);
            Boolean guessed = false;
            int counter = 0;
            while (!guessed || counter==10 ){
                agent2.sendMessage(await agent2.generateAsyncAnswerOnly("Your Notes: " + agent2.getNotes() + "Ask your next question or guess the character: "),agent1);
                Trace.WriteLine("Question: "+agent2.convlist.Last().message);
                agent2.setNotes(agent2.getNotes()+ agent2.convlist.Last().message);
                agent1.sendMessage(await agent2.generateAsyncAnswerOnly(" Your character is: "+agent1.getNotes()+" The Question is: "+agent2.convlist.Last().message+ "answer with \"Yes\" or \"No\" or \"GAME OVER\" only. Answer \"GAME OVER\" only if your Character is guessed correctly."),agent2);
                Trace.WriteLine("Answer: "+agent2.convlist.Last().message);
                agent2.setNotes(agent2.getNotes()+": "+ agent2.convlist.Last().message+"--");
                if(agent2.convlist.Last().message== "GAME OVER"){
                    guessed=true;
                }
                counter++;
            }
            
            Assert.True(guessed);
            
        
        }

    }

}
