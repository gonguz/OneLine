using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
/// <summary>
/// Class Level is used for saving each level read from the .json
/// </summary>
public class Level
{
    //index of the level
    public int index { get; set; }

    //layout of the level
    public string[] layout { get; set; }

    //path to complete the level
    public List<int[]> path { get; set; }
}


/// <summary>
/// Tool class that allows us to read all levels of a certain category, and saves them in a list which is returned
/// </summary>
public class LevelReader
{
    //List where levels will be saved
    private List<Level> _levels;

    /// <summary>
    /// Creates an empty list of levels.
    /// </summary>
    public LevelReader() {
        _levels = new List<Level>();
    }

    /// <summary>
    /// This method reads the maps of a given category, which are read previously from
    /// the Resources folder. Is loaded as a TextAsset, and readed as a text when we want to read
    /// all its content. Once all this is done, the filled list is returned.
    /// </summary>
    /// <param name="category">category from where we want to get the levels
    public List<Level> CreateLevels(GameManager.LEVEL_CATEGORY category)
    {
        _levels.Clear();
        TextAsset map = (TextAsset)Resources.Load("Maps/" + category, typeof(TextAsset));
        
        _levels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Level>>(map.text);

        return _levels;

    }
}
