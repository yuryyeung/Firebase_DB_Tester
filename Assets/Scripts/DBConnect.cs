using Firebase;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

public class DBConnect
{
    public UnityEvent OnFirebaseInitialized = new UnityEvent();
    public RealtimeDB RealtimeDB;
    public FirestoreDB FirestoreDB;

    public DBConnect()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        { 
            if (task.Exception != null)
            {
                Debug.LogError($"Failed to initialize Firebase With {task.Exception}");
                return;
            }
            Debug.Log("Firebase initialized");
            OnFirebaseInitialized.Invoke();

            RealtimeDB = new RealtimeDB();
            FirestoreDB = new FirestoreDB();
        });
    }

    public void Dispose()
    {
        RealtimeDB.Dispose();
    }
}


public class Players
{
    public Player[] player;
    public Players(Player[] player) { this.player = player; }
}