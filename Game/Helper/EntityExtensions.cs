using Stride.Engine;

namespace TestLoading.Helper
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Recursively searches for a component of type T in an entity and all its child entities
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="entity">Entity with component or child Entities</param>
        /// <returns>EntityComponent of type T or null</returns>
        public static T GetComponentInChildren<T>(this Entity entity) where T : EntityComponent
        {
            if (entity == null) return null;

            T component = entity.Get<T>();
            if (component != null)
                return component;

            foreach (var child in entity.GetChildren())
            {
                component = child.GetComponentInChildren<T>();
                if (component != null)
                    return component;
            }

            return null;
        }
    }
}