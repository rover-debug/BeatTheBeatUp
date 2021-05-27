using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvReader
{
    Queue<string> mLines;
    Dictionary<string, int> mHeaders;
    string[] currentLine;
    int mLineNumber = 0;
    public CsvReader(string file)
    {
        mHeaders = new Dictionary<string, int>();

        mLines = new Queue<string>(File.ReadAllLines(file));
        string[] headers = mLines.Dequeue().Split(',');
        int idx = 0;
        foreach(var header in headers)
        {
            mHeaders.Add(header, idx++);
        }
        mLineNumber = 1;
    }

    public CsvReader(string CsvData, bool Text=false)
    {
        mLines = new Queue<string>(CsvData.Split('\n'));
        mHeaders = new Dictionary<string, int>();

       ////Debug.Log(mLines.Peek());
        string[] headers = mLines.Dequeue().Split(',');
        int idx = 0;

       ////Debug.Log(headers.Length);
        
        for (;idx < headers.Length;)
        {
            if (idx == headers.Length-1)
            {
                headers[idx] = headers[idx].Substring(0, headers[idx].Length-1);
            }
            mHeaders.Add(headers[idx], idx++);
        }
        mLineNumber = 1;
    }

    public bool Read()
    {
        if (mLines.Count > 0)
        {
            currentLine = mLines.Dequeue().Split(',');
            ++mLineNumber;
            return true;
        }
        return false;
    }

    public int GetHeaderIndex(string head)
    {
        if (!mHeaders.ContainsKey(head))
            return -1;
        return mHeaders[head];
    }

    public string GetFieldOrEmpty(string head)
    {
        // foreach (string key in currentLine)
        // {
        //    ////Debug.Log(key);
        // }
        //////Debug.Log(head);
        //////Debug.Log(mHeaders[head]);
        if (!mHeaders.ContainsKey(head))
            return "";
        return currentLine[mHeaders[head]];
    }

    public int GetLineNumber()
    {
        return mLineNumber;
    }
}
