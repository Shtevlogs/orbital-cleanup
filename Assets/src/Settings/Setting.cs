using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting<T> where T : IComparable
{
    private T _value = default(T);
    public T Value { get { return _value; }
        set {
            if (Comparer<T>.Default.Compare(value, _value) == 0)
            {
                return;
            }

            _value = value;
            OnSettingChanged?.Invoke(_value);
        }
    }

    public delegate void OnSettingChangedDelegate(T newSetting);
    public OnSettingChangedDelegate OnSettingChanged;

    public Setting(T def = default(T)){
        _value = def;
    }
}
