﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

public class XMLSerialiser 
{
	public enum DirectoryTarget
	{
		DESKTOP,
		APPLICATION,
		USER
	}

	/* The following metods came from the referenced URL */
	public static string UTF8ByteArrayToString(byte[] characters)
	{
		UTF8Encoding encoding = new UTF8Encoding();
		string constructedString = encoding.GetString(characters);
		return (constructedString);
	}
	
	public static byte[] StringToUTF8ByteArray(string pXmlString)
	{
		UTF8Encoding encoding = new UTF8Encoding();
		byte[] byteArray = encoding.GetBytes(pXmlString);
		return byteArray;
	}

	// Sserialize our data object 
	public static string SerializeObject(object pObject)
	{

		System.Type type = pObject.GetType();
		string XmlizedString = null;
		MemoryStream memoryStream = new MemoryStream();
		XmlSerializer xs = new XmlSerializer(type);


        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);

		memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

		return XmlizedString;
	}

    public static void SerializeObjectBin(string path, object pObject)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

        bf.Serialize(fs, pObject);

        fs.Close();
    }

    public static string SerializeToString(string path, object pObject)
    {
        using (MemoryStream s = new MemoryStream())
        {
            new BinaryFormatter().Serialize(s, pObject);
            string str = Convert.ToBase64String(s.ToArray());

#if UNITY_WEBPLAYER
#else
            CreateXML(path, str);
#endif
            return str;
        } 
    }

    public static object DeserializeObjectBin(string path, bool isResource)
    {
        object obj = null;

        BinaryFormatter bf = new BinaryFormatter();

        if (!isResource)
        {

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

            obj = bf.Deserialize(fs);

            fs.Close();
        }
        else
        {
            TextAsset textAsset = Resources.Load(path) as TextAsset;
            if (textAsset == null)
            {
                Debug.Log(path);
            }
            Stream stream = new MemoryStream(textAsset.bytes);

            obj = bf.Deserialize(stream);

            stream.Close();
        }

        return obj;
    }


	// Deserialize it back to its original form 
	public static object DeserializeObject(string pXmlizedString, string myType)
	{
		// Convert the data type
		System.Type type = System.Type.GetType(myType);

		// Standard xml serializer library
		XmlSerializer xs = new XmlSerializer(type);

		// Create a memory stream and pass the xml serialized string
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));

		// Return deserialized memory stream
		return xs.Deserialize(memoryStream);
	}

	private static string GetTargetDirectory(DirectoryTarget target)
	{
		string path = "";

		switch(target)
		{
			case DirectoryTarget.DESKTOP:
				{
					path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
				}
				break;
			case DirectoryTarget.APPLICATION:
				{
					path = Application.persistentDataPath;
				}
				break;
			case DirectoryTarget.USER:
				{
					path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
				}
				break;
		}

		return path;
	}

#if UNITY_WEBPLAYER
#else
	public static void CreateXML(DirectoryTarget target, string path, string _FileName, string _data)
	{
		string _FileLocation = GetTargetDirectory(target) + "\\" + path;

		// Check if directory exists
		if (!System.IO.Directory.Exists(_FileLocation))
		{
			// It doesn't so make it
			
			System.IO.Directory.CreateDirectory(_FileLocation);
		}

		StreamWriter writer;
		FileInfo t = new FileInfo(_FileLocation + "\\" + _FileName);

		// Check if XML file exists
		if (!t.Exists)
		{
			// It doesn't so make it
			writer = t.CreateText();
		}
		else
		{
			t.Delete();
			writer = t.CreateText();
		}
		writer.Write(_data);
		writer.Close();
	}

	public static void CreateXML(string filePath, string data)
	{
		StreamWriter writer;

		FileInfo t = new FileInfo(filePath);

		// Check if XML file exists
		if (!t.Exists)
		{
			// It doesn't so make it
			writer = t.CreateText();
		}
		else
		{
			t.Delete();
			writer = t.CreateText();
		}
		writer.Write(data);
		writer.Close();
	}
#endif


	public static string LoadXML(DirectoryTarget target, string path, string _FileName)
	{
		string _FileLocation = GetTargetDirectory(target) + "\\" + path;
		StreamReader r;
		r = File.OpenText(_FileLocation + "\\" + _FileName);
		string _info = r.ReadToEnd();
		r.Close();
		return _info;
	}

	public static string LoadXML(string filePath)
	{
		StreamReader r;
		r = File.OpenText(filePath);
		string _info = r.ReadToEnd();
		r.Close();
		return _info;
	} 
}
