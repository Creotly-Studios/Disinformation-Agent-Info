using System;
using UnityEngine;

public interface IHasProgress
{
    public event EventHandler OnProgressChangedEvent;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float normalizedProgressValue;
    }
}
