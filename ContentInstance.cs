using Mantodea.Content;
using Mantodea.Content.Players;
using Mantodea.Content.Rooms;
using Mantodea.Content.Tiles;
using Mantodea.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Mantodea
{
    public class ContentInstance<T> where T : GameContent
    {
        public static T Instance { get; internal set; }

        private static List<T> _instances = new List<T>();

        public static IReadOnlyList<T> Instances { get => _instances.AsReadOnly(); private set => _instances = (List<T>)value; }
    
        public static bool OneInstance()
        {
            return Instance is Player || Instance is Room;
        }

        static ContentInstance()
        {
            Instance = Activator.CreateInstance<T>();

            Instance.SetStaticDefaults();

            Instance.SetDefaults();
        }

        public static T NewInstance()
        {
            if (OneInstance() && Instance != null)
                return Instance;

            Instance = Activator.CreateInstance<T>();

            Instance.SetDefaults();

            _instances.Add(Instance);

            return Instance;
        }

        public static Entity NewEntity(Vector2 drawOffset = default)
        {
            var instance = NewInstance();

            if (instance is Entity entity)
            {
                entity.DrawOffset = drawOffset;
                return entity;
            }
            else return null;
        }

        public static T NewTile(int subID = 1, Vector2 drawOffset = default, string texID = "")
        {
            var instance = NewEntity(drawOffset);
            if (instance is Tile tile)
            {
                tile.SubID = subID;
                if (texID != "")
                {
                    tile.Texture = Main.TextureManager[TexType.Tile, texID];
                    tile.TexturePath = texID;
                }
                return tile as T;
            }
            else return null;
        }
    }
}
