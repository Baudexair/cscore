using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using com.csutil.keyvaluestore;

namespace com.csutil.tests.keyvaluestore {

    public class MockDelayKeyValueStore : IKeyValueStore {

        public IKeyValueStore fallbackStore { get; set; }
        public long latestFallbackGetTimingInMs { get; set; }

        public int delay = 100;
        public bool throwTimeoutError = false;

        public DisposeState IsDisposed { get; private set; } = DisposeState.Active;

        public void Dispose() {
            IsDisposed = DisposeState.DisposingStarted;
            fallbackStore?.Dispose();
            IsDisposed = DisposeState.Disposed;
        }

        public MockDelayKeyValueStore(IKeyValueStore fallbackStore) { this.fallbackStore = fallbackStore; }

        private async Task SimulateDelay() {
            await TaskV2.Delay(delay);
            if (throwTimeoutError) { throw new TimeoutException("Simulated error"); }
        }

        public async Task<bool> ContainsKey(string key) {
            await SimulateDelay();
            return await fallbackStore.ContainsKey(key);
        }

        public async Task<T> Get<T>(string key, T defaultValue) {
            await SimulateDelay();
            var s = Stopwatch.StartNew();
            var result = await fallbackStore.Get<T>(key, defaultValue);
            latestFallbackGetTimingInMs = s.ElapsedMilliseconds;
            return result.DeepCopyViaJson();
        }

        public async Task<IEnumerable<string>> GetAllKeys() {
            await SimulateDelay();
            return await fallbackStore.GetAllKeys();
        }

        public async Task<bool> Remove(string key) {
            await SimulateDelay();
            return await fallbackStore.Remove(key);
        }

        public async Task RemoveAll() {
            await SimulateDelay();
            await fallbackStore.RemoveAll();
        }

        public async Task<object> Set(string key, object value) {
            await SimulateDelay();
            var result = await fallbackStore.Set(key, value.DeepCopyViaJson());
            return result.DeepCopyViaJson();
        }

    }

}