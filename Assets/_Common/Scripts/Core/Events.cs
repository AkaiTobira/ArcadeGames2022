using System.Collections.Generic;

public enum GameplayEventType{
    ResizeAsteroids,
    RecalculateTerrain,
    RecalculateDiggers,
    GameOver,
    RefreshRanking,
    RefreshConections,
    EnemyHasBeenMurdered,
    LocalizationUpdate,
    ButtonOvervieved,
    SaveRankings,
    ContinueAnimation,
    PlayerDied,
    SpamWithWindow,
    SpawnFollower,
    RecolorPlayer,
    ReverseAnimation, //sick idea of client
}

public static class Events{
    public static GameEventSystem<GameplayEvent, GameplayEventType> Gameplay = 
        new GameEventSystem<GameplayEvent, GameplayEventType>();
}


#region GameplayGameEventSystem

    public class GameplayEvent : GameEvent<GameplayEventType>{

        public GameplayEvent(){}
        public GameplayEvent( GameplayEventType _type): base(_type){}

        public GameplayEvent( GameplayEventType _type, object _value): base(_type, _value){}
    };

    public interface IListenToGameplayEvents : IListenToEvents<GameplayEvent, GameplayEventType>{};

#endregion

