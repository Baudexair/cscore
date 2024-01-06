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
            Trace.WriteLine("Hello, World!");
            Log.d(answer + " Review:" + re);
        }

        [Fact]
        public async Task Test3_Agentguesser()
        {
            string key="";
            Agent agent1 = new Agent("Agent1", "You will be playing a guessing game. You will have to answer the questions with yes, no or maybe until your character is guessed. If your character is guessed only write: Game Over. Your character is: ", key);
            Agent agent2 = new Agent("Agent2", "You will be playing a guessing game. Ask questions until you correctly guess the character. Your previous questions and their answers will be given to you: ", key);
            string answer = await agent1.generateAsyncAnswerOnly("Choose a character and write only the character down.");
            agent1.setNotes(answer);
            Boolean guessed = false;
            int counter = 0;
            while (!guessed || counter==10 ){
                await Task.Delay(3000);
                agent2.sendMessage(await agent2.generateAsyncAnswerOnly("Your Notes: " + agent2.getNotes() + "Ask your next question or guess the character: "),agent1);
                agent2.setNotes(agent2.getNotes()+ agent2.convlist.Last().message);
                await Task.Delay(3000);
                agent1.sendMessage(await agent2.generateAsyncAnswerOnly(agent1.getNotes()+agent2.convlist.Last().message),agent2);
                agent2.setNotes(agent2.getNotes()+": "+ agent2.convlist.Last().message+"--");
                if(agent2.convlist.Last().message== "Game Over"){
                    guessed=true;
                }
                counter++;
            }
            
            Assert.True(guessed);
            
        
        }

    }

}
