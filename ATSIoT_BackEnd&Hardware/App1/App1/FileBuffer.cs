using System;

public class FileBuffer
{
	public FileBuffer()
	{

	}

    public static void writeToFile(string messageString)
    {
        Task taskA = Task.Run(() => WriteToTextFile(tempFileName, messageString));
        taskA.Wait();
    }

    public static string readFile(string fileName)
    {
        Task<string> taskReadFile = Task.Run(() => ReadAndClearTextFile(fileName));
        taskReadFile.Wait();
        return taskReadFile.Result;
    }

    private static async Task<StorageFile> openFile(string fileName)
    {
        return await storageFolder.CreateFileAsync(fileName,
            CreationCollisionOption.OpenIfExists);
    }

    private static async Task WriteToTextFile(string fileName, string messageString)
    {
        Task<StorageFile> taskOpenFile = Task.Run(() => openFile(fileName));
        StorageFile file = taskOpenFile.Result;

        await FileIO.AppendTextAsync(file, messageString + "\n");

    }

    private static async Task<string> ReadAndClearTextFile(string filename)
    {
        string contents;

        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        StorageFile textFile = await localFolder.GetFileAsync(filename);

        using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
        {
            using (DataReader textReader = new DataReader(textStream))
            {
                uint textLength = (uint)textStream.Size;
                await textReader.LoadAsync(textLength);
                contents = textReader.ReadString(textLength);
            }
        }
        return contents;
    }
}
