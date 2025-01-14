using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectCommon
{
    public enum ListEditType
    {
        Add,
        Remove,
        swap,
        clear,
    }

    public enum NameTagLayer
    {
        Name,
        Tag,
        Layer,

    }
    
    public enum TagLayer
    {
        Tag,
        Layer,

    }


    public static GameObject FindChildInSameParent(GameObject CurrentObject, string NameOrTagOrLayer, NameTagLayer SearchType)
    {
        GameObject parent = CurrentObject.transform.parent.gameObject;
        foreach (Transform transform in parent.transform)
        {
            if (SearchType == NameTagLayer.Name)
            {
                if (transform.gameObject.name == NameOrTagOrLayer)
                {
                    return transform.gameObject;
                }
            }
            if (SearchType == NameTagLayer.Tag)
            {
                if (transform.gameObject.CompareTag(NameOrTagOrLayer))
                {
                    return transform.gameObject;
                }
            }
            if (SearchType == NameTagLayer.Layer)
            {
                if (transform.gameObject.layer == LayerMask.NameToLayer(NameOrTagOrLayer))
                {
                    return transform.gameObject;
                }
            }

        }

        return null;
    }
    
    public static GameObject FindChildwithTagStringLayer(GameObject CurrentObject, string NameOrTagOrLayer, NameTagLayer SearchType)
    {

        foreach (Transform transform in CurrentObject.transform)
        {
            if (SearchType == NameTagLayer.Name)
            {
                if (transform.gameObject.name == NameOrTagOrLayer)
                {
                    return transform.gameObject;
                }
            }
            if (SearchType == NameTagLayer.Tag)
            {
                if (transform.gameObject.CompareTag(NameOrTagOrLayer))
                {
                    return transform.gameObject;
                }
            }
            if (SearchType == NameTagLayer.Layer)
            {
                if (transform.gameObject.layer == LayerMask.NameToLayer(NameOrTagOrLayer))
                {
                    return transform.gameObject;
                }
            }

        }

        return null;    
    }
    
    public static List<GameObject> FindChildrenWithTagLayer(GameObject ParentOBJ, string TagLayer, TagLayer SearchType)
    {
        List<GameObject> ChildList = new List<GameObject>();
        foreach (Transform transform in ParentOBJ.transform)
        {
            if (SearchType == GameObjectCommon.TagLayer.Tag)
            {
                if (transform.gameObject.CompareTag(TagLayer))
                {
                    ChildList.Add(transform.gameObject);
                }
            }
            if (SearchType == GameObjectCommon.TagLayer.Layer)
            {
                if (transform.gameObject.layer == LayerMask.NameToLayer(TagLayer))
                {
                    ChildList.Add(transform.gameObject);
                }
            }

        }

        return ChildList;
    }

    public static List<GameObject> GetAllChildren(GameObject ParentOBJ)
    {
        List<GameObject> ChildList = new List<GameObject>();

        foreach (Transform transform in ParentOBJ.transform)
        {
            ChildList.Add(transform.gameObject);
        }
        return ChildList;
    }
    public static List<GameObject> GetAllChildrenRecursive(GameObject ParentOBJ)
    {
        List<GameObject> ChildList = new List<GameObject>();

        foreach (Transform transform in ParentOBJ.transform)
        {
            ChildList.Add(transform.gameObject);

            ChildList.AddRange(GetAllChildren(transform.gameObject));

        }



        return ChildList;
    }



    public static List<GameObject> EditingGameObjectList(ListEditType AddOrRemove, Collider other, List<GameObject> List)
    {


        if (AddOrRemove == ListEditType.Add)
        {
            List.Add(other.gameObject);
            return List;
        }

        if (AddOrRemove == ListEditType.Remove)
        {
            List.Remove(other.gameObject);
            return List;
        }

        return null;
    }


    public static List<GameObject> EditingGOListByTag(ListEditType AddOrRemove, Collider other, List<GameObject> List, string Tag)
    {

        if (other.gameObject.CompareTag(Tag)) { return EditingGameObjectList(AddOrRemove, other, List); }

        return (List);

    }

    public static List<GameObject> EditingGOListByLayer(ListEditType AddOrRemove, Collider other, List<GameObject> List, string LayerName)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(LayerName)) { return List; }

        return EditingGameObjectList(AddOrRemove, other, List);
    }

    public static List<GameObject> EditingGOListByLayer(ListEditType AddOrRemove, Collider other, List<GameObject> List, int LayerNumber)
    {
        if (other.gameObject.layer != LayerNumber) { return List; }

        return EditingGameObjectList(AddOrRemove, other, List);
    }
}
