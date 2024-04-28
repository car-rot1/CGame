using System;
using UnityEngine;
using CGame;
using CGame.Localization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Direction = CGame.Direction;

[Serializable]
public class CsvData
{
    [AnotherName("数量")] public int num;
    private string name;
    public bool IsActive { get; set; }

    public CsvData()
    {
        
    }

    public CsvData(int num, string name)
    {
        this.num = num;
        this.name = name;
    }

    public override string ToString()
    {
        return "num : " + num + " name : " + name + " IsActive : " + IsActive;
    }

    public void Init()
    {
        Debug.Log("初始化");
    }

    public void DeInit()
    {
        Debug.Log("反初始化");
    }
}

public struct JsonTest
{
    public string id;
    public string text;
}

public abstract class DataBase
{
    public static readonly Data0 Data0 = new();
    public static readonly Data1 Data1 = new();
    public static readonly Data2 Data2 = new();
    public static readonly Data3 Data3 = new();

    public abstract DataBase Method0(object value);
    public abstract DataBase Method1(object value);
}

public class Data0 : DataBase
{
    public override DataBase Method0(object value)
    {
        //value xxxxxx
        return Data1;
    }
    
    public override DataBase Method1(object value)
    {
        //value xxxxxx
        return Data0;
    }
}

public class Data1 : DataBase
{
    public override DataBase Method0(object value)
    {
        //value xxxxxx
        return Data1;
    }
    
    public override DataBase Method1(object value)
    {
        //value xxxxxx
        return Data1;
    }
}

public class Data2 : DataBase
{
    public override DataBase Method0(object value)
    {
        //value xxxxxx
        return Data2;
    }
    
    public override DataBase Method1(object value)
    {
        //value xxxxxx
        return Data3;
    }
}

public class Data3 : DataBase
{
    public override DataBase Method0(object value)
    {
        //value xxxxxx
        return Data1;
    }
    
    public override DataBase Method1(object value)
    {
        //value xxxxxx
        return Data1;
    }
}

public class User
{
    private object[] _objects = new object[10];

    public void Use(int c = 0, int b = 1)
    {
        
    }
    public void Use(int a = 1)
    {
        DataBase data = DataBase.Data0;
        foreach (var o in _objects)
        {
            if (true)
            {
                data = data.Method0(o);
            }
            else
            {
                data = data.Method1(o);
            }
        }
    }
}

public class Test : MonoBehaviour
{
    public Dropdown dropDown;

    private void Awake()
    {
        if (dropDown == null)
            return;
        
        var index = 0;
        
        dropDown.options.Clear();
        var system = LocalizationSystem.Instance;
        for (var i = 0; i < system.internalLanguages.Count; i++)
        {
            var internalLanguage = system.internalLanguages[i];
            if (internalLanguage.Equals(system.Language))
                index = i;
            dropDown.options.Add(new Dropdown.OptionData(internalLanguage));
        }
        foreach (var externalLanguage in system.externalLanguages)
        {
            dropDown.options.Add(new Dropdown.OptionData(externalLanguage));
        }

        dropDown.value = index;
        dropDown.onValueChanged.AddListener(i =>
        {
            system.Language = dropDown.options[i].text;
        });
    }
}