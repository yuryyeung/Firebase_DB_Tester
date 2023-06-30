using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    DBConnect db;
    [SerializeField] UIView view;

    [SerializeField] public Player player;
    [SerializeField] public List<Card> cardList = new List<Card>();

    [SerializeField] public Player uploadPlayer;
    [SerializeField] public Player[] uploadPlayers;

    public string FireStoreDoc = "PlayerData";

    // Start is called before the first frame update
    void Start()
    {
        db = new DBConnect();
        view.playerBtn.onClick.AddListener(() => { StartCoroutine(loadplayers()); });

        view.uploadPlayer.onClick.AddListener(() => { db.RealtimeDB.SavePlayerData(uploadPlayer); });
        view.uploadPlayers.onClick.AddListener(() => { db.RealtimeDB.SavePlayersData(uploadPlayers); });

        view.playerFirestoreRNDBtn.onClick.AddListener(() => { StartCoroutine(LoadAll()); });
        view.playerFirestoreBtn.onClick.AddListener(() => { StartCoroutine(LoadSpecific()); });
        view.uploadPlayerList.onClick.AddListener(() => {
            Players players = new Players(uploadPlayers);
            var json = JsonUtility.ToJson(players);
            Debug.Log(json);
            db.FirestoreDB.AddData(json, "GameData", "PlayerList");
        });
        view.uploadRNDPlayers.onClick.AddListener(() => {
            var json = JsonUtility.ToJson(uploadPlayer);
            Debug.Log(json);
            db.FirestoreDB.AddData(json, "GameData");
        });
    }

    public void Update()
    {
        if (!string.IsNullOrEmpty(db.RealtimeDB.RealtimeResponse))
            view.realtimeText.text = db.RealtimeDB.RealtimeResponse;
    }

    public IEnumerator LoadAll()
    {
        view.responseText.text = string.Empty;
        var player = db.FirestoreDB.GetData("GameData");
        yield return new WaitUntil(() => player.IsCompleted);
        foreach (string players in player.Result)
        {
            view.responseText.text += players + "\n";
        }
    }

    public IEnumerator LoadSpecific()
    {
        view.responseText.text = string.Empty;
        var player = db.FirestoreDB.GetData("GameData", FireStoreDoc);
        yield return new WaitUntil(() => player.IsCompleted);
        foreach (string players in player.Result)
        {
            view.responseText.text += players + "\n";
        }
    }

    public void OnDestroy()
    {
        db.Dispose();
    }

    IEnumerator loadplayer()
    {
        var player = db.RealtimeDB.GetPlayer();
        yield return new WaitUntil(() => player.IsCompleted);
        var result = player.Result;
        if (player != null)
        {
            view.responseText.text = $"player: [\n" +
                $"name: {result.Name}" +
                $"id: {result.ID}" +
                $"card: {result.Cards}" +
                $"]";
        }
    }

    IEnumerator loadplayers()
    {
        var player = db.RealtimeDB.GetRawData("player");
        yield return new WaitUntil(() => player.IsCompleted);
        var result = player.Result;
        if (player != null)
            view.responseText.text = result;
    }
}
