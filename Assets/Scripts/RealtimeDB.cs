using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RealtimeDB
{
    FirebaseDatabase _rdb;

    private string _responseRaw;
    public string RealtimeResponse { get { return _responseRaw; } }

    public RealtimeDB()
    {
        _rdb = FirebaseDatabase.DefaultInstance;
        _rdb.RootReference.ValueChanged += ValueChanged;
    }

    public void Dispose()
    {
        _rdb.RootReference.ValueChanged -= ValueChanged;
    }

    public void ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var json = e.Snapshot.GetRawJsonValue();
        _responseRaw = json;
    }

    public void SavePlayerData(Player player)
    {
        _rdb.GetReference("player")
            .SetRawJsonValueAsync(JsonUtility.ToJson(player));
    }

    public void SavePlayersData(Player[] player)
    {
        Players players = new Players(player);
        Debug.Log(JsonUtility.ToJson(players));
        _rdb.GetReference("player")
            .SetRawJsonValueAsync(JsonUtility.ToJson(players));
    }

    public async Task<Player> GetPlayer()
    {
        var dbsnapshot = await _rdb.GetReference("player").GetValueAsync();
        if (!dbsnapshot.Exists) { Debug.Log("Database Not Exist"); return null; }
        return JsonUtility.FromJson<Player>(dbsnapshot.GetRawJsonValue());
    }

    public async Task<string> GetRawData(string key)
    {
        var dbsnapshot = await _rdb.GetReference(key).GetValueAsync();
        if (!dbsnapshot.Exists) return null;
        return dbsnapshot.GetRawJsonValue();
    }

    public void SaveCardData(Card card)
    {
        _rdb.GetReference("card")
           .SetRawJsonValueAsync(JsonUtility.ToJson(card));
    }

    public async Task<bool> SaveExists(string key)
    {
        var dbSnapshot = await _rdb.GetReference(key).GetValueAsync();
        return dbSnapshot.Exists;
    }

    public void EraseData(string key)
    {
        _rdb.GetReference(key).RemoveValueAsync();
    }
}
