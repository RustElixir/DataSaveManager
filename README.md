# DataSaveManager
simple system of saving and loading data

## Install

### Install DataSaveManager
Window -> Package Manager -> AddPackage fom git url

https://github.com/elicxir/DataSaveManager.git

## How to Use

### DataHandler

DataHandler is a class for handling data in selected files.

By specifying the path of a file, you can obtain a DataHandler for that file, and you can read and write data in the file through the DataHandler.

### Get DataHandler

SaveManager.Load() can be used to obtain the DataHandler for a specified file in the folder Application.persistentDataPath.

If the file does not exist, a file will be created.
```cs
  //Get DataHandler for Application.persistentDataPath/xxx.txt
  var dh= DataSaveManager.SaveManager.Load("xxx.txt");
```

### Storing and Restoring Data
DataHandler has the concept of a key, and data can be exchanged based on this key.

### How to store data

Store TestData type data in DataHandler using the key "data_key".
Execute Save() of DataHandler with the data stored to save the changes to a file.

Don't forget to add System.Serializable attribute to the class you want to save. 

```cs
[System.Serializable]
public class TestData{
  public int value;
}
```

```cs
  TestData data=new TestData();
  data.value=3156;

  var dh= DataSaveManager.SaveManager.Load("xxx.txt");
  dh.SetData<TestData>("data_key", data);
  dh.Save();
```

### How to restore data
Restore TestData type data from DataHandler using the key "data_key".

Note that data cannot be retrieved unless key and type match.

In that case, KeyNotFoundException will be thrown, so you can use try catch to determine if the data was retrieved or not.
```cs
  var dh= DataSaveManager.SaveManager.Load("xxx.txt");
  TestData data;

  try
  {
        data=dh.GetData<TestData>("data_key");
  } 
  catch (KeyNotFoundException e)
  {
      Console.WriteLine("key not found");
  }

```

### SaveManager
|  Functions | Description |
| ---- | ---- |
|DataHandler Load(string filename)|Get the DataHandler for the specified file.<br>Specify the name of a file under the Application.persistentDataPath folder|
|void Delete(string filename)|Deletes the specified file.<br>Specify the name of a file under the Application.persistentDataPath folder|
### DataHandler

|  Functions | Description |
| ---- | ---- |
|T GetData<T>(string key)|Retrieve data from DataHandler based on key.|
|bool SetData<T>(string key, T obj)|Stores data in DataHandler using key and class instance<br>Returns true if an existing value is updated, or false if a new value is stored.|
|void Save()|Save the DataHandler to a file.|


### DataType Restrection
These may not work.

- Dictionary<>
- List<List<>>
- property (get,set)