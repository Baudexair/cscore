﻿using com.csutil.logging;
using com.csutil.testing;
using com.csutil.tests.http;
using com.csutil.tests.model;
using com.csutil.tests.model.immutable;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.csutil.tests {

    /// <summary> Tests the Assert class and other reimplemented Xunit classes </summary>
    public class XunitTestRunnerTests {

        [UnityTest]
        public IEnumerator RunXunitTest_DataStoreExample2() {
            yield return RunTestsInClass(typeof(DataStoreExample2));
        }

        [UnityTest]
        public IEnumerator RunXunitTest_FeatureFlagTests() {
            yield return RunTestsInClass(typeof(FeatureFlagTests));
        }

        [UnityTest]
        public IEnumerator RunXunitTest_RestTests() {
            yield return RunTestsInClass(typeof(RestTests));
        }

        //[UnityTest]
        public IEnumerator RunAllXunitTest() {
            var allClasses = typeof(MathTests).Assembly.GetExportedTypes();
            foreach (var classToTest in allClasses) {
                yield return RunTestsInClass(classToTest);
            }
        }

        private IEnumerator RunTestsInClass(Type classToTest) {
            var allTests = XunitTestRunner.GetIteratorOverAllTests(classToTest, delegate {
                // setup before each test
                Log.instance = new LogForXunitTestRunnerInUnity();
            });
            foreach (var test in allTests) { yield return StartTest(test); }
        }

        private IEnumerator StartTest(XunitTestRunner.Test runningTest) {
            var t = Log.MethodEntered("XunitTestRunnerTests.StartTest", "Now running test " + runningTest);
            yield return new WaitForSeconds(0.1f);
            runningTest.StartTest();
            yield return runningTest.testTask.AsCoroutine((e) => { Debug.LogWarning(e); }, timeoutInMs: 60000);
            Log.MethodDone(t);
            if (runningTest.testTask.IsFaulted) {
                Debug.LogWarning("Error in test " + runningTest);
                yield return new WaitForSeconds(0.1f);
                var ex = runningTest.reportedError?.SourceException;
                if (ex == null) { ex = runningTest.testTask.Exception; }
                Log.w("" + runningTest, ex);
                Debug.LogError(ex);
                yield return new WaitForSeconds(0.1f);
                runningTest.reportedError.Throw();
            }
        }

    }

}
