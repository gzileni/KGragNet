using System;

namespace KGragNet
{

    public class KGragStore
    {
        private KGragQDrantConfig qdrantConfig = new KGragQDrantConfig();
        private KGragGraphConfig graphConfig = new KGragGraphConfig();
        public KGragQdrant Vector;
        public KGragGraph Graph;

        /*
         * Default constructor generates a new ThreadId.
         */
        public KGragStore()
        {
        
        }

        public KGragStore(KGragQDrantConfig qdrantConfig) : this()
        {
            QdrantConfig = qdrantConfig;
        }

        public KGragStore(KGragQDrantConfig qdrantConfig, KGragGraphConfig graphConfig) : this(qdrantConfig)
        {
            GraphConfig = graphConfig;
        }

        /*
         * QdrantConfig property to get or set the KGragQDrantConfig.
         * Setting this property will also initialize the Vector property with a new KGragQdrant instance
         * and call the Create method to set up the collection.
         */
        public KGragQDrantConfig QdrantConfig
        {
            get => qdrantConfig;
            set
            {
                qdrantConfig = value;
                Vector = new KGragQdrant(value);
                // Esegui il metodo asincrono in modo sincrono
                var createMethod = Vector.GetType().GetMethod("Create");
                if (createMethod != null)
                {
                    var task = (System.Threading.Tasks.Task)createMethod.Invoke(Vector, new object[] { value.CollectionName, value.VectorSize, value.Distance });
                    task.GetAwaiter().GetResult();
                }
            }
        }

        /*
         * GraphConfig property to get or set the KGragGraphConfig.
         * Setting this property will also initialize the Graph property with a new KGragGraph instance.
         */
        public KGragGraphConfig GraphConfig
        {
            get => graphConfig;
            set
            {
                graphConfig = value;
                Graph = new KGragGraph(value);
            }
        }
    }
}
