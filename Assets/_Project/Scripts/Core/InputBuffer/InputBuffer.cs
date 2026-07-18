using System;
using System.Collections.Generic;
using UnityEngine;
using GyeNyame.Core.Events;

namespace GyeNyame.Core.InputBuffer
{
    public class InputBuffer : MonoBehaviour
    {
        private readonly List<BufferedCommand> _commands = new();

        private void Update() => CleanExpiredCommands();

        public void BufferCommand<T>(float bufferTime) where T : IMessage
        {
            var type = typeof(T);
            var currentTime = Time.time;

            for (int i = 0; i < _commands.Count; i++)
            {
                if (_commands[i].MessageType == type)
                {
                    _commands[i] = new BufferedCommand(type, currentTime, currentTime + bufferTime);
                    return;
                }
            }

            _commands.Add(new BufferedCommand(type, currentTime, currentTime + bufferTime));
        }

        public bool HasCommand<T>() where T : IMessage
        {
            var type = typeof(T);
            var currentTime = Time.time;
            
            for (int i = 0; i < _commands.Count; i++)
            {
                if (_commands[i].MessageType == type && _commands[i].IsValid(currentTime))
                {
                    return true;
                }
            }
            return false;
        }

        public void ConsumeCommand<T>() where T : IMessage
        {
            var type = typeof(T);
            
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                if (_commands[i].MessageType == type)
                {
                    _commands.RemoveAt(i);
                    break;
                }
            }
        }

        public void Clear()
        {
            _commands.Clear();
        }

        private void CleanExpiredCommands()
        {
            float currentTime = Time.time;
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                if (!_commands[i].IsValid(currentTime))
                {
                    _commands.RemoveAt(i);
                }
            }
        }
    }
}
