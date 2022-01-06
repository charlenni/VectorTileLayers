using System.Collections.Generic;
using System.Text;

namespace Mapsui.VectorTileLayer.Core.Primitives
{
    /// <summary>
    /// Represents a simple tags collection based on a dictionary.
    /// </summary>
    public class TagsCollection
    {
        private const char KeyValueSeparator = '=';

        /// The key of the house number OpenStreetMap tag
        public const string KeyHouseNumber = "addr:housenumber";

        /// The key of the name OpenStreetMap tag
        public const string KeyName = "name";

        /// The key of the reference OpenStreetMap tag
        public const string KeyRef = "ref";

        /// The key of the elevation OpenStreetMap tag
        public const string KeyEle = "ele";

        /// The key of the id tag
        public const string KeyId = "id";

        public const string KeyAmenity = "amenity";
        public const string KeyBuilding = "building";
        public const string KeyHighway = "highway";
        public const string KeyLanduse = "landuse";

        public const string ValueYes = "yes";
        public const string ValueNo = "no";

        // S3DB
        public const string KeyArea = "area";
        public const string KeyBuildingColor = "building:colour";
        public const string KeyBuildingLevels = "building:levels";
        public const string KeyBuildingMaterial = "building:material";
        public const string KeyBuildingMinLevel = "building:min_level";
        public const string KeyBuildingPart = "building:part";
        //public const string KEY_COLOR = "colour";
        public const string KeyHeight = "height";
        //public const string KEY_MATERIAL = "material";
        public const string KeyMinHeight = "min_height";
        public const string KeyRoof = "roof";
        public const string KeyRoofAngle = "roof:angle";
        public const string KeyRoofColor = "roof:colour";
        public const string KeyRoofDirection = "roof:direction";
        public const string KeyRoofHeight = "roof:height";
        public const string KeyRoofLevels = "roof:levels";
        public const string KeyRoofMaterial = "roof:material";
        public const string KeyRoofOrientation = "roof:orientation";
        public const string KeyRoofShape = "roof:shape";
        public const string KeyVolume = "volume";

        // Roof shape values
        public const string ValueDome = "dome";
        public const string ValueFlat = "flat";
        public const string ValueGabled = "gabled";
        public const string ValueGambrel = "gambrel";
        public const string ValueHalfHipped = "half_hipped";
        public const string ValueHipped = "hipped";
        public const string ValueMansard = "mansard";
        public const string ValueOnion = "onion";
        public const string ValuePyramidal = "pyramidal";
        public const string ValueRound = "round";
        public const string ValueSaltbox = "saltbox";
        public const string ValueSkillion = "skillion";

        public const string ValueAcross = "across"; // orientation across

        List<string> _keys = new List<string>();
        List<object> _values = new List<object>();

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection(params KeyValuePair<string, object>[] tags)
        {
            foreach (var tag in tags)
                Add(tag.Key, tag.Value);
        }

        /// <summary>
        /// Creates a new tags collection initialized with the given existing tags.
        /// </summary>
        /// <param name="tags"></param>
        public TagsCollection(IEnumerable<KeyValuePair<string, object>> tags)
        {
            foreach (var tag in tags)
                Add(tag.Key, tag.Value);
        }

        /// <summary>
        /// Creates a new tags collection initialized with the given existing tags.
        /// </summary>
        /// <param name="tags"></param>
        public TagsCollection(IDictionary<string, string> tags)
        {
            if (tags != null)
            {
                foreach (KeyValuePair<string, string> pair in tags)
                {
                    Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Number of elements in this tags collection
        /// </summary>
        public int Count { get => _keys.Count; }

        public object this[string key]
        {
            get
            {
                int index = _keys.IndexOf(key);
                if (index < 0)
                    return null;
                return _values[index];
            }
            set
            {
                Add(key, value);
            }
        }

        public IEnumerable<KeyValuePair<string, object>> KeyValues
        {
            get
            {
                var result = new List<KeyValuePair<string, object>>(Count);

                for (int i = 0; i < _keys.Count; i++)
                    result.Add(new KeyValuePair<string, object>(_keys[i], _values[i]));

                return result;
            }
        }

        public void Clear()
        {
            _keys.Clear();
            _values.Clear();
        }

        /// <summary>
        /// Adds a tag from a string with key-value-separator
        /// </summary>
        /// <param name="tag">String of key-value-pair separated with key-value-separator</param>
        public void Add(string tag)
        {
            var splitPosition = tag.IndexOf(KeyValueSeparator);

            Add(tag.Substring(0, splitPosition), tag.Substring(splitPosition + 1));
        }

        /// <summary>
        /// Adds a list of tags to this collection.
        /// </summary>
        /// <param name="tags">List of tags</param>
        public void Add(IEnumerable<KeyValuePair<string, object>> tags)
        {
            foreach (var tag in tags)
                Add(tag.Key, tag.Value);
        }

        /// <summary>
        /// Adds a list of tags to this collection.
        /// </summary>
        /// <param name="tags">List of tags</param>
        public void Add(TagsCollection tags)
        {
            foreach (var tag in tags.KeyValues)
                Add(tag.Key, tag.Value);
        }

        /// <summary>
        /// Add a key value pair to the TagsCollection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            int index = _keys.IndexOf(key);

            if (index < 0)
            {
                _keys.Add(key);
                _values.Add(value);
            }
            else
            {
                _values[index] = value;
            }
        }

        /// <summary>
        /// Returns true, if the tags collection contains given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _keys.Contains(key);
        }

        /// <summary>
        /// Returns true if the given key-value pair is found in this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsKeyValue(string key, object value)
        {
            int index = _keys.IndexOf(key);

            if (index < 0 || _values[index] == null)
                return false;

            return _values[index].Equals(value);
        }

        /// <summary>
        /// Returns true if one of the given keys exists in this tag collection.
        /// </summary>
        /// <param name="keys">Collection of keys to check</param>
        /// <returns>True, if one key in keys is containd in this collection</returns>
        public virtual bool ContainsOneOfKeys(ICollection<string> others)
        {
            foreach (var other in others)
            {
                if (_keys.Contains(other))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all tags with given key and value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool RemoveKeyValue(string key, object value)
        {
            if (ContainsKeyValue(key, value))
            {
                int index = _keys.IndexOf(key);
                _keys.RemoveAt(index);
                _values.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check for equality
        /// </summary>
        /// <param name="other">Other TagCollections to check</param>
        /// <returns>True, if it is the same TagsCollection or contains the same tags</returns>
        public override bool Equals(object other)
        {
            if (other == this)
                return true;

            if (!(other is TagsCollection otherTagsCollection) || otherTagsCollection.Count != _keys.Count)
                return false;

            for (int i = 0; i < _keys.Count; i++)
                if (!otherTagsCollection.ContainsKeyValue(_keys[i], _values[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns a string that represents this tags collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder tags = new StringBuilder();
            for (int i = 0; i < _keys.Count; i++)
            {
                tags.Append($"{_keys[i]}{KeyValueSeparator}{_values[i]}");
                tags.Append(',');
            }
            if (tags.Length > 0)
            {
                return tags.ToString(0, tags.Length - 1);
            }
            return "empty";
        }
    }
}