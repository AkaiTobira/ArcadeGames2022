using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent<T> where T : System.Enum
{
    public T type;
    public object parameter;

    public GameEvent( T _type, object _parameter ){
        parameter = _parameter;
        type = _type;
    }

    public GameEvent( T _type){
        parameter = null;
        type = _type;
    }
};

public interface IListenToEvents<G, T>
    where T : System.Enum
    where G : GameEvent<T> {
    void OnGameEvent( G gameEvent );
}

public class GameEventSystem<G, T> 
    where G : GameEvent<T>
    where T : System.Enum
{
    Dictionary< T, List<IListenToEvents<G,T>>> _registeredObjects = new Dictionary<T, List<IListenToEvents<G, T>>>(); 
    List<IListenToEvents<G,T>> _listenersToRemove = new List<IListenToEvents<G, T>>();

    public void RegisterListener( IListenToEvents<G,T> newListener, T type){
        if( !_registeredObjects.ContainsKey(type) ){
            _registeredObjects[type] = new List<IListenToEvents<G,T>>(){ newListener };
        }else{
            if(_registeredObjects[type].Contains(newListener)) return;
            _registeredObjects[type].Add(newListener);
        }
    }

    public void DeregisterListener( IListenToEvents<G,T> newListener, T type ){
        if( _registeredObjects.TryGetValue( type, out List<IListenToEvents<G,T>> list ) ){
            list.Remove( newListener );
        }
    }

    public void RiseEvent( G gameEvent ){
        Debug.Log("GameEvent rised : " + gameEvent.type);
        if( _registeredObjects.TryGetValue( gameEvent.type, out List<IListenToEvents<G,T>> list ) ){
            for( int i = 0; i < list.Count; i++){
                IListenToEvents<G,T> listener = list[i];

                //Destroyed object security tests 
                if(listener is UnityEngine.MonoBehaviour){
                    if(Guard.IsValid(listener as UnityEngine.MonoBehaviour)){
                        listener.OnGameEvent(gameEvent);
                    }else{
                        _listenersToRemove.Add(listener);
                        Debug.Log("Unregistered unexisting listener");
                    }
                }else{
                    listener?.OnGameEvent(gameEvent);
                }
            }

            for(int i = 0; i < _listenersToRemove.Count; i++) {
                DeregisterListener(_listenersToRemove[i], gameEvent.type);   
            }
            _listenersToRemove.Clear();
        }
    }
}

