using Firebase.Extensions;
using Firebase.Firestore;
using Kogane;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Text.Json;
using System.Linq;

public class FirestoreDB : IDisposable
{
    FirebaseFirestore _fdb;

    public FirestoreDB()
    {
        _fdb = FirebaseFirestore.DefaultInstance;
    }

    public void AddData(string data, string collection, string document = null)
    {
        //var json = JsonToLibary.ParseJson(data);
        var dictreq = JsonUtility.FromJson<JsonDictionary<string, object>>(data);
        var req = dictreq.Dictionary;
        string doc = string.IsNullOrEmpty(document) ? DateTime.Now.ToString("yyyyMMddhhmmss") : document;
        DocumentReference df = _fdb.Collection(collection).Document(doc);
        df.SetAsync(req).ContinueWithOnMainThread(task => {
            Debug.Log(JsonUtility.ToJson(req));
            Debug.Log("Added data to the alovelace document in the users collection.");
        });
    }

    public async Task<List<string>> GetData(string collection, string document = null)
    {
        List<string> result = new List<string>();
        DocumentReference cf = _fdb.Collection(collection).Document(document);

        await cf.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            CardList cardList = snapshot.ConvertTo<CardList>();
            if (snapshot.Exists)
            {
                Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                foreach (Card card in cardList.card)
                {
                    //KeyValuePair<string, string> rt = new KeyValuePair<string, string>("", cardData);
                    //Debug.Log(String.Format("{0}: {1}", pair.Key, cardData));
                    string jsonString = JsonUtility.ToJson(card);
                    if (document == null) result.Add(jsonString);
                    else if (snapshot.Id.Contains(document)) result.Add(jsonString);
                }


                //Dictionary<string, object> city = snapshot.ToDictionary();
                //foreach (KeyValuePair<string, object> pair in city)
                //{
                //    Card card = (Card)pair.Value;
                //    string cardData = JsonUtility.ToJson(card);
                //    KeyValuePair<string, string> rt = new KeyValuePair<string, string>(pair.Key, cardData);
                //    Debug.Log(String.Format("{0}: {1}", pair.Key, cardData));
                //    if (document == null) result.Add(DictionaryReturns(rt));
                //    else if (snapshot.Id.Contains(document)) result.Add(DictionaryReturns(rt));
                //}

            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
        return result;
    }

    public string DictionaryReturns (KeyValuePair<string, string> value)
    {
        return $"[{value.Key}: {value.Value}]";
    }

    public static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
        return dictionary;
    }

    public void Dispose()
    {
        
    }
}