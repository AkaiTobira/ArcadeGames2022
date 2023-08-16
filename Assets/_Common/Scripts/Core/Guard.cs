using UnityEngine;

public static class Guard
{
    public static bool IsValid(object obj){
        return obj != null;
    }
    public static bool IsValid( GameObject obj ){
        return obj != null && !ReferenceEquals(obj, null);
    }
    public static bool IsValid( Component obj ){
        return obj != null && !ReferenceEquals(obj, null);
    }
}
