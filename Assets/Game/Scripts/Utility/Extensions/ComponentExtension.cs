using UnityEngine;

namespace Garawell.Extensions
{
    public static class ComponentExtension
    {
        public static T[] GetComponentsInChildrensWithoutParent<T>(this Component component)
        {
            T[] childrens = component.GetComponentsInChildren<T>();
            T[] withoutParents = new T[childrens.Length - 1];
            for (int i = 0; i < withoutParents.Length; i++)
            {
                withoutParents[i] = childrens[i + 1];
            }

            return withoutParents;
        }
    }
}
