using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.csutil.http.apis;
using com.csutil.model.jsonschema;
using Newtonsoft.Json.Linq;
using Xunit;

namespace com.csutil.integrationTests.http {
    public class OpenAiAgentTests{

        [Fact]
        public void Test1_Agentreview()
        {
            Agent agent1 = new Agent("Agent1", "Give an answer to the given prompt:", "Key");
            Agent agent2 = new Agent("Agent2", "You will be given a prompt and an answer to this prompt. Verify if the answer is correct and grade it on a scale from 1 to 10, with 10 being the best.  :", "Key");
            string answer = agent1.generateAsync("When did WW2 end?").ToString();
            agent1.sendMessage(answer, agent2);
            string re =  agent2.generateAsync(agent2.convlist.Last().message).ToString();

            Assert.NotEmpty(re);
            Log.d(answer + " Review:" + re);
        }

    }

}
