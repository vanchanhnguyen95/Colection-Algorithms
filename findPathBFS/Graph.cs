using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace findPathBFS
{
    public interface IBaseVertex
	{
		int get_id();
		double get_weight();
		void set_weight(double weight);
	}

	public interface IBaseEdge
	{
		int get_weight();

		Vertex get_start_vertex();
		Vertex get_end_vertex();
	}

	public interface IBaseElementWithWeight
	{
		double get_weight();
	}

	public interface IBaseGraph
	{
		List<Vertex> get_vertex_list();

		double get_edge_weight(Vertex source, Vertex sink);
		HashSet<Vertex> get_adjacent_vertices(Vertex vertex);
		HashSet<Vertex> get_precedent_vertices(Vertex vertex);
	}

	public class Vertex : IBaseVertex, IComparable<Vertex>
	{
		public static int CURRENT_VERTEX_NUM = 0;
		private int _id = CURRENT_VERTEX_NUM++;
		private double _weight = 0;

		/**
	    * 
	    */
		public int get_id()
		{
			return _id;
		}

		public override string ToString()
		{
			return " " + _id;
		}

		public double get_weight()
		{
			return _weight;
		}

		public void set_weight(double status)
		{
			_weight = status;
		}

		public int CompareTo(Vertex r_vertex)
		{
			double diff = this._weight - r_vertex._weight;
			if (diff > 0)
				return 1;
			else if (diff < 0)
				return -1;
			else
				return 0;
		}
	}

	public class Path : IBaseElementWithWeight
	{
		List<Vertex> _vertex_list;
		double _weight = -1;

		public Path()
		{
			_vertex_list = new List<Vertex>();
		}

		public Path(List<Vertex> _vertex_list, double _weight)
		{
			this._vertex_list = _vertex_list;
			this._weight = _weight;
		}

		public double get_weight()
		{
			return _weight;
		}

		public void set_weight(double weight)
		{
			_weight = weight;
		}

		public List<Vertex> get_vertices()
		{
			return _vertex_list;
		}

		public override int GetHashCode()
		{
			return _vertex_list.GetHashCode();
		}

		public override bool Equals(Object right)
		{
			if (right is Path)
			{
				Path r_path = right as Path;
				return _vertex_list.Equals(r_path._vertex_list);
			}
			return false;
		}

		public override string ToString()
		{
			//return _vertex_list.ToString() + ":" + _weight;
			string s = "Path = ";
			foreach (Vertex vertex in _vertex_list)
			{
				s += vertex.ToString();
			}
			return s + " : " + _weight;
		}
	}
	public class Graph : IBaseGraph
	{
		public static double DISCONNECTED;

		// index of fan-outs of one vertex
		protected Dictionary<int, HashSet<Vertex>> _fanout_vertices_index;

		// index for fan-ins of one vertex
		protected Dictionary<int, HashSet<Vertex>> _fanin_vertices_index;

		// index for edge weights in the graph
		protected Dictionary<KeyValuePair<int, int>, double> _vertex_pair_weight_index;

		// index for vertices in the graph
		protected Dictionary<int, Vertex> _id_vertex_index;

		// list of vertices in the graph 
		protected List<Vertex> _vertex_list;

		// the number of vertices in the graph
		public int _vertex_num;

		// the number of arcs in the graph
		public int _edge_num;

		/**
	    * Constructor 1 
	    * @param data_file_name
	    */
		public Graph(string data_file_name)
		{

			DISCONNECTED = double.MaxValue;

			// index of fan-outs of one vertex
			_fanout_vertices_index = new Dictionary<int, HashSet<Vertex>>();

			// index for fan-ins of one vertex
			_fanin_vertices_index = new Dictionary<int, HashSet<Vertex>>();

			// index for edge weights in the graph
			_vertex_pair_weight_index = new Dictionary<KeyValuePair<int, int>, double>();

			// index for vertices in the graph
			_id_vertex_index = new Dictionary<int, Vertex>();

			// list of vertices in the graph 
			_vertex_list = new List<Vertex>();

			// the number of vertices in the graph
			_vertex_num = 0;

			// the number of arcs in the graph
			_edge_num = 0;
			import_from_file(data_file_name);
		}

		/**
	    * Constructor 2
	    * 
	    * @param graph
	    */
		public Graph(Graph graph_)
		{
			DISCONNECTED = double.MaxValue;

			// index of fan-outs of one vertex
			_fanout_vertices_index = new Dictionary<int, HashSet<Vertex>>(graph_._fanout_vertices_index);

			// index for fan-ins of one vertex
			_fanin_vertices_index = new Dictionary<int, HashSet<Vertex>>(graph_._fanin_vertices_index);

			// index for edge weights in the graph
			_vertex_pair_weight_index = new Dictionary<KeyValuePair<int, int>, double>(graph_._vertex_pair_weight_index);

			// index for vertices in the graph
			_id_vertex_index = new Dictionary<int, Vertex>(graph_._id_vertex_index);

			// list of vertices in the graph 
			_vertex_list = new List<Vertex>(graph_._vertex_list);

			// the number of vertices in the graph
			_vertex_num = graph_._vertex_num;

			// the number of arcs in the graph
			_edge_num = graph_._edge_num;
		}

		/**
	    * Default constructor 
	    */
		public Graph()
		{
			DISCONNECTED = double.MaxValue;

			// index of fan-outs of one vertex
			_fanout_vertices_index = new Dictionary<int, HashSet<Vertex>>();

			// index for fan-ins of one vertex
			_fanin_vertices_index = new Dictionary<int, HashSet<Vertex>>();

			// index for edge weights in the graph
			_vertex_pair_weight_index = new Dictionary<KeyValuePair<int, int>, double>();

			// index for vertices in the graph
			_id_vertex_index = new Dictionary<int, Vertex>();

			// list of vertices in the graph 
			_vertex_list = new List<Vertex>();

			// the number of vertices in the graph
			_vertex_num = 0;

			// the number of arcs in the graph
			_edge_num = 0;
		}

		/**
	    * Clear members of the graph.
	    */
		public void Clear()
		{
			_vertex_num = 0;
			_edge_num = 0;
			_vertex_list.Clear();
			_id_vertex_index.Clear();
			_fanin_vertices_index.Clear();
			_fanout_vertices_index.Clear();
			_vertex_pair_weight_index.Clear();
		}

		/**
	    * There is a requirement for the input graph. 
	    * The ids of vertices must be consecutive. 
	    *  
	    * @param data_file_name
	    */
		public void import_from_file(string data_file_name)
		{
			try
			{
				// 1. read the file and put the content in the buffer
				TextReader bufRead = new StreamReader(data_file_name);

				bool is_first_line = true;
				string line;    // String that holds current file line

				// 2. Read first line
				line = bufRead.ReadLine();
				while (line != null)
				{
					// 2.1 skip the empty line
					if (line.Trim().Equals(""))
					{
						line = bufRead.ReadLine();
						continue;
					}

					// 2.2 generate nodes and edges for the graph
					if (is_first_line)
					{
						//2.2.1 obtain the number of nodes in the graph 
						Vertex.CURRENT_VERTEX_NUM = 0;
						is_first_line = false;
						_vertex_num = int.Parse(line.Trim()) + 1;
						for (int i = 0; i < _vertex_num; ++i)
						{
							Vertex vertex = new Vertex();
							_vertex_list.Add(vertex);
							_id_vertex_index[vertex.get_id()] = vertex;
						}

					}
					else
					{
						//2.2.2 find a new edge and put it in the graph  
						string[] str_list = line.Trim().Split('\r', '\n', '\t', ' ');

						int start_vertex_id = int.Parse(str_list[0]);
						int end_vertex_id = int.Parse(str_list[1]);
						double weight = double.Parse(str_list[2]);
						add_edge(start_vertex_id, end_vertex_id, weight);
					}
					//
					line = bufRead.ReadLine();
				}
				bufRead.Close();

			}
			catch (IOException e)
			{
				// If another exception is generated, print a stack trace
				Console.Write(e.Message);
			}
		}

		/**
	    * Note that this may not be used externally, because some other members in the class
	    * should be updated at the same time. 
	    * 
	    * @param start_vertex_id
	    * @param end_vertex_id
	    * @param weight
	    */
		protected void add_edge(int start_vertex_id, int end_vertex_id, double weight)
		{
			// actually, we should make sure all vertices ids must be correct. 
			if (!_id_vertex_index.ContainsKey(start_vertex_id)
			|| !_id_vertex_index.ContainsKey(end_vertex_id)
			|| start_vertex_id == end_vertex_id)
			{
				throw new System.ArgumentOutOfRangeException("The edge from " + start_vertex_id
						+ " to " + end_vertex_id + " does not exist in the graph.");
			}

			// update the adjacent-list of the graph
			HashSet<Vertex> fanout_vertex_set;
			if (_fanout_vertices_index.ContainsKey(start_vertex_id))
			{
				fanout_vertex_set = _fanout_vertices_index[start_vertex_id];
			}
			else
			{
				fanout_vertex_set = new HashSet<Vertex>();
				_fanout_vertices_index.Add(start_vertex_id, fanout_vertex_set);
			}
			fanout_vertex_set.Add(_id_vertex_index[end_vertex_id]);

			//
			HashSet<Vertex> fanin_vertex_set;
			if (_fanin_vertices_index.ContainsKey(end_vertex_id))
			{
				fanin_vertex_set = _fanin_vertices_index[end_vertex_id];
			}
			else
			{
				fanin_vertex_set = new HashSet<Vertex>();
				_fanin_vertices_index.Add(end_vertex_id, fanin_vertex_set);
			}
			fanin_vertex_set.Add(_id_vertex_index[start_vertex_id]);

			// store the new edge 
			_vertex_pair_weight_index.Add(new KeyValuePair<int, int>(start_vertex_id, end_vertex_id),
				weight);

			++_edge_num;
		}

		/**
	    * Store the graph information into a file. 
	    * 
	    * @param file_name
	    */
		public void export_to_file(string file_name)
		{
			try
			{
				//1. prepare the text to export
				TextWriter tw = new StreamWriter(file_name);
				tw.WriteLine(_vertex_num);
				foreach (KeyValuePair<KeyValuePair<int, int>, double> cur_edge_pair in _vertex_pair_weight_index)
				{
					KeyValuePair<int, int> cur_edge = cur_edge_pair.Key;
					int starting_pt_id = cur_edge.Key;
					int ending_pt_id = cur_edge.Value;
					double weight = cur_edge_pair.Value;
					tw.WriteLine(starting_pt_id + "\t" + ending_pt_id + "\t" + weight);
				}
				tw.Close();
			}
			catch (IOException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		/* (non-Javadoc)
	    * @see edu.asu.emit.qyan.alg.model.abstracts.BaseGraph#get_adjacent_vertices(edu.asu.emit.qyan.alg.model.abstracts.BaseVertex)
	    */
		public virtual HashSet<Vertex> get_adjacent_vertices(Vertex vertex)
		{
			return _fanout_vertices_index.ContainsKey(vertex.get_id())
				? _fanout_vertices_index[vertex.get_id()]
				: new HashSet<Vertex>();
		}

		/* (non-Javadoc)
	    * @see edu.asu.emit.qyan.alg.model.abstracts.BaseGraph#get_precedent_vertices(edu.asu.emit.qyan.alg.model.abstracts.BaseVertex)
	    */
		public virtual HashSet<Vertex> get_precedent_vertices(Vertex vertex)
		{
			return _fanin_vertices_index.ContainsKey(vertex.get_id())
				? _fanin_vertices_index[vertex.get_id()]
				: new HashSet<Vertex>();
		}

		/* (non-Javadoc)
	    * @see edu.asu.emit.qyan.alg.model.abstracts.BaseGraph#get_edge_weight(edu.asu.emit.qyan.alg.model.abstracts.BaseVertex, edu.asu.emit.qyan.alg.model.abstracts.BaseVertex)
	    */
		public virtual double get_edge_weight(Vertex source, Vertex sink)
		{
			KeyValuePair<int, int> Key = new KeyValuePair<int, int>(source.get_id(), sink.get_id());
			return (_vertex_pair_weight_index.ContainsKey(Key)) ?
				_vertex_pair_weight_index[Key] : DISCONNECTED;
		}

		/**
	    * Set the number of vertices in the graph
	    * @param num
	    */
		public virtual void set_vertex_num(int num)
		{
			_vertex_num = num;
		}

		/**
	    * Return the vertex list in the graph.
	    */
		public virtual List<Vertex> get_vertex_list()
		{
			return _vertex_list;
		}

		/**
	    * Get the vertex with the input id.
	    * 
	    * @param id
	    * @return
	    */
		public virtual Vertex get_vertex(int id)
		{
			return _id_vertex_index[id];
		}
	}

	public class VariableGraph : Graph
	{
		HashSet<int> _rem_vertex_id_set;
		HashSet<KeyValuePair<int, int>> _rem_edge_set;

		/**
	    * Default constructor
	    */
		public VariableGraph()
		{
			_rem_vertex_id_set = new HashSet<int>();
			_rem_edge_set = new HashSet<KeyValuePair<int, int>>();
		}

		/**
	    * Constructor 1
	    * 
	    * @param data_file_name
	    */
		public VariableGraph(string data_file_name)
			: base(data_file_name)
		{
			_rem_vertex_id_set = new HashSet<int>();
			_rem_edge_set = new HashSet<KeyValuePair<int, int>>();
		}

		/**
	    * Constructor 2
	    * 
	    * @param graph
	    */
		public VariableGraph(Graph graph)
			: base(graph)
		{
			_rem_vertex_id_set = new HashSet<int>();
			_rem_edge_set = new HashSet<KeyValuePair<int, int>>();
		}

		/**
	    * Set the set of vertices to be removed from the graph
	    * 
	    * @param _rem_vertex_list
	    */
		public void set_rem_vertex_id_list(ICollection<int> _rem_vertex_list)
		{
			this._rem_vertex_id_set.UnionWith(_rem_vertex_list);
		}

		/**
	    * Set the set of edges to be removed from the graph
	    * 
	    * @param _rem_edge_hashcode_set
	    */
		public void set_rem_edge_hashcode_set(ICollection<KeyValuePair<int, int>> rem_edge_collection)
		{
			_rem_edge_set.UnionWith(rem_edge_collection);
		}

		/**
	    * Add an edge to the set of removed edges
	    * 
	    * @param edge
	    */
		public void remove_edge(KeyValuePair<int, int> edge)
		{
			_rem_edge_set.Add(edge);
		}

		/**
	    * Add a vertex to the set of removed vertices
	    * 
	    * @param vertex_id
	    */
		public void remove_vertex(int vertex_id)
		{
			_rem_vertex_id_set.Add(vertex_id);
		}

		public void recover_removed_edges()
		{
			_rem_edge_set.Clear();
		}

		public void recover_removed_edge(KeyValuePair<int, int> edge)
		{
			_rem_edge_set.Remove(edge);
		}

		public void recover_removed_vertices()
		{
			_rem_vertex_id_set.Clear();
		}

		public void recover_removed_vertex(int vertex_id)
		{
			_rem_vertex_id_set.Remove(vertex_id);
		}

		/**
	    * Return the weight associated with the input edge.
	    * 
	    * @param source
	    * @param sink
	    * @return
	    */
		public override double get_edge_weight(Vertex source, Vertex sink)
		{
			int source_id = source.get_id();
			int sink_id = sink.get_id();

			if (_rem_vertex_id_set.Contains(source_id) || _rem_vertex_id_set.Contains(sink_id)
				|| _rem_edge_set.Contains(new KeyValuePair<int, int>(source_id, sink_id)))
			{
				return Graph.DISCONNECTED;
			}
			return base.get_edge_weight(source, sink);
		}

		/**
	    * Return the weight associated with the input edge.
	    * 
	    * @param source
	    * @param sink
	    * @return
	    */
		public double get_edge_weight_of_graph(Vertex source, Vertex sink)
		{
			return base.get_edge_weight(source, sink);
		}

		/**
	    * Return the set of fan-outs of the input vertex.
	    * 
	    * @param vertex
	    * @return
	    */
		public override HashSet<Vertex> get_adjacent_vertices(Vertex vertex)
		{
			HashSet<Vertex> ret_set = new HashSet<Vertex>();
			int starting_vertex_id = vertex.get_id();
			if (!_rem_vertex_id_set.Contains(starting_vertex_id))
			{
				HashSet<Vertex> adj_vertex_set = base.get_adjacent_vertices(vertex);
				foreach (Vertex cur_vertex in adj_vertex_set)
				{
					int ending_vertex_id = cur_vertex.get_id();
					if (_rem_vertex_id_set.Contains(ending_vertex_id)
					|| _rem_edge_set.Contains(
						new KeyValuePair<int, int>(starting_vertex_id, ending_vertex_id)))
					{
						continue;
					}

					// 
					ret_set.Add(cur_vertex);
				}
			}
			return ret_set;
		}

		/**
	    * Get the set of vertices preceding the input vertex.
	    * 
	    * @param vertex
	    * @return
	    */
		public override HashSet<Vertex> get_precedent_vertices(Vertex vertex)
		{
			HashSet<Vertex> ret_set = new HashSet<Vertex>();
			if (!_rem_vertex_id_set.Contains(vertex.get_id()))
			{
				int ending_vertex_id = vertex.get_id();
				HashSet<Vertex> pre_vertex_set = base.get_precedent_vertices(vertex);
				foreach (Vertex cur_vertex in pre_vertex_set)
				{
					int starting_vertex_id = cur_vertex.get_id();
					if (_rem_vertex_id_set.Contains(starting_vertex_id)
					|| _rem_edge_set.Contains(
						new KeyValuePair<int, int>(starting_vertex_id, ending_vertex_id)))
					{
						continue;
					}

					//
					ret_set.Add(cur_vertex);
				}
			}
			return ret_set;
		}

		/**
	    * Get the list of vertices in the graph, except those removed.
	    * @return
	    */
		public override List<Vertex> get_vertex_list()
		{
			List<Vertex> ret_list = new List<Vertex>();
			foreach (Vertex cur_vertex in base.get_vertex_list())
			{
				if (_rem_vertex_id_set.Contains(cur_vertex.get_id())) continue;
				ret_list.Add(cur_vertex);
			}
			return ret_list;
		}

		/**
	    * Get the vertex corresponding to the input 'id', if exist. 
	    * 
	    * @param id
	    * @return
	    */
		public override Vertex get_vertex(int id)
		{
			if (_rem_vertex_id_set.Contains(id))
			{
				return null;
			}
			else
			{
				return base.get_vertex(id);
			}
		}
	}


	public class QYPriorityQueue<T> where T : IBaseElementWithWeight
	{
		List<T> _element_weight_pair_list;
		int _limit_size = -1;
		bool _is_incremental = false;

		/**
	    * Default constructor. 
	    */
		public QYPriorityQueue()
		{
			_element_weight_pair_list = new List<T>();
		}

		/**
	    * Constructor. 
	    * @param limit_size
	    */
		public QYPriorityQueue(int limit_size, bool is_incremental)
		{
			_limit_size = limit_size;
			_is_incremental = is_incremental;
		}

		/* (non-Javadoc)
	    * @see java.lang.Object#toString()
	    */
		public override string ToString()
		{
			return _element_weight_pair_list.ToString();
		}

		/**
	    * Binary search is exploited to find the right position 
	    * of the new element. 
	    * @param weight
	    * @return the position of the new element
	    */
		private int _bin_locate_pos(double weight, bool is_incremental)
		{
			int mid = 0;
			int low = 0;
			int high = _element_weight_pair_list.Count - 1;
			//
			while (low <= high)
			{
				mid = (low + high) / 2;
				if (_element_weight_pair_list.ElementAt(mid).get_weight() == weight)
					return mid + 1;

				if (is_incremental)
				{
					if (_element_weight_pair_list.ElementAt(mid).get_weight() < weight)
					{
						high = mid - 1;
					}
					else
					{
						low = mid + 1;
					}
				}
				else
				{
					if (_element_weight_pair_list.ElementAt(mid).get_weight() > weight)
					{
						high = mid - 1;
					}
					else
					{
						low = mid + 1;
					}
				}
			}
			return low;
		}

		/**
	    * Add a new element in the queue. 
	    * @param element
	    */
		public void add(T element)
		{
			_element_weight_pair_list.Insert(_bin_locate_pos(element.get_weight(), _is_incremental), element);

			if (_limit_size > 0 && _element_weight_pair_list.Count > _limit_size)
			{
				int size_of_results = _element_weight_pair_list.Count;
				_element_weight_pair_list.RemoveAt(size_of_results - 1);
			}
		}

		/**
	    * It only reflects the size of the current results.
	    * @return
	    */
		public int size()
		{
			return _element_weight_pair_list.Count;
		}

		/**
	    * Get the i th element. 
	    * @param i
	    * @return
	    */
		public T get(int i)
		{
			if (i >= _element_weight_pair_list.Count)
			{
				Console.WriteLine("The result :" + i + " doesn't exist!!!");
			}
			return _element_weight_pair_list[i];
		}

		/**
	    * Get the first element, and then remove it from the queue. 
	    * @return
	    */
		public T poll()
		{
			T ret = _element_weight_pair_list[0];
			_element_weight_pair_list.RemoveAt(0);
			return ret;
		}

		/**
	    * Check if it's empty.
	    * @return
	    */
		public bool isEmpty()
		{
			return (_element_weight_pair_list.Count == 0);
		}

		public void Clear()
		{
			_element_weight_pair_list.Clear();
		}
	}


	public class VPriorityQueue<T> where T : IBaseVertex
	{
		List<T> _element_weight_pair_list;
		int _limit_size = -1;
		bool _is_incremental = false;

		/**
        * Default constructor. 
        */
		public VPriorityQueue()
		{
			_element_weight_pair_list = new List<T>();
		}

		/**
        * Constructor. 
        * @param limit_size
        */
		public VPriorityQueue(int limit_size, bool is_incremental)
		{
			_limit_size = limit_size;
			_is_incremental = is_incremental;
		}

		/* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
		public override string ToString()
		{
			return _element_weight_pair_list.ToString();
		}

		/**
        * Binary search is exploited to find the right position 
        * of the new element. 
        * @param weight
        * @return the position of the new element
        */
		private int _bin_locate_pos(double weight, bool is_incremental)
		{
			int mid = 0;
			int low = 0;
			int high = _element_weight_pair_list.Count - 1;
			//
			while (low <= high)
			{
				mid = (low + high) / 2;
				if (_element_weight_pair_list.ElementAt(mid).get_weight() == weight)
					return mid + 1;

				if (is_incremental)
				{
					if (_element_weight_pair_list.ElementAt(mid).get_weight() < weight)
					{
						high = mid - 1;
					}
					else
					{
						low = mid + 1;
					}
				}
				else
				{
					if (_element_weight_pair_list.ElementAt(mid).get_weight() > weight)
					{
						high = mid - 1;
					}
					else
					{
						low = mid + 1;
					}
				}
			}
			return low;
		}

		/**
        * Add a new element in the queue. 
        * @param element
        */
		public void Add(T element)
		{
			_element_weight_pair_list.Insert(_bin_locate_pos(element.get_weight(), _is_incremental), element);

			if (_limit_size > 0 && _element_weight_pair_list.Count > _limit_size)
			{
				int size_of_results = _element_weight_pair_list.Count;
				_element_weight_pair_list.RemoveAt(size_of_results - 1);
			}
		}

		/**
        * It only reflects the size of the current results.
        * @return
        */
		public int Size()
		{
			return _element_weight_pair_list.Count;
		}

		/**
        * Get the i th element. 
        * @param i
        * @return
        */
		public T Get(int i)
		{
			if (i >= _element_weight_pair_list.Count)
			{
				Console.WriteLine("The result :" + i + " doesn't exist!!!");
			}
			return _element_weight_pair_list[i];
		}

		/**
        * Get the first element, and then remove it from the queue. 
        * @return
        */
		public T Poll()
		{
			T ret = _element_weight_pair_list[0];
			_element_weight_pair_list.RemoveAt(0);
			return ret;
		}

		/**
        * Check if it's empty.
        * @return
        */
		public bool IsEmpty()
		{
			return (_element_weight_pair_list.Count == 0);
		}

		public void Clear()
		{
			_element_weight_pair_list.Clear();
		}
	}

	public class DijkstraShortestPathAlg
	{
		// Input
		Graph _graph = null;

		// Intermediate variables
		HashSet<Vertex> _determined_vertex_set;
		VPriorityQueue<Vertex> _vertex_candidate_queue;
		Dictionary<Vertex, Double> _start_vertex_distance_index;
		Dictionary<Vertex, Vertex> _predecessor_index;

		/**
	    * Default constructor.
	    * @param graph
	    */
		public DijkstraShortestPathAlg(Graph graph)
		{
			_determined_vertex_set = new HashSet<Vertex>();
			_vertex_candidate_queue = new VPriorityQueue<Vertex>();
			_start_vertex_distance_index = new Dictionary<Vertex, double>();
			_predecessor_index = new Dictionary<Vertex, Vertex>();
			_graph = graph;
		}

		/**
	    * Clear intermediate variables.
	    */
		public void clear()
		{
			_determined_vertex_set.Clear();
			_vertex_candidate_queue.Clear();
			_start_vertex_distance_index.Clear();
			_predecessor_index.Clear();
		}

		public Dictionary<Vertex, double> get_start_vertex_distance_index()
		{
			return _start_vertex_distance_index;
		}

		public Dictionary<Vertex, Vertex> get_predecessor_index()
		{
			return _predecessor_index;
		}

		/**
	    * Construct a tree rooted at "root" with 
	    * the shortest paths to the other vertices.
	    * 
        * @param root
	     */
		public void get_shortest_path_tree(Vertex root)
		{
			determine_shortest_paths(root, null, true);
		}

		/**
	    * Construct a flower rooted at "root" with 
	    * the shortest paths from the other vertices.
	    * 
	    * @param root
	    */
		public void get_shortest_path_flower(Vertex root)
		{
			determine_shortest_paths(null, root, false);
		}

		/**
	    * Do the work
	    */
		protected void determine_shortest_paths(Vertex source_vertex,
			Vertex sink_vertex, bool is_source2sink)
		{
			// 0. clean up variables
			clear();

			// 1. initialize members
			Vertex end_vertex = is_source2sink ? sink_vertex : source_vertex;
			Vertex start_vertex = is_source2sink ? source_vertex : sink_vertex;
			_start_vertex_distance_index[start_vertex] = 0.0;
			start_vertex.set_weight(0.0);
			_vertex_candidate_queue.Add(start_vertex);

			// 2. start searching for the shortest path
			while (!_vertex_candidate_queue.IsEmpty())
			{
				Vertex cur_candidate = _vertex_candidate_queue.Poll();

				if (cur_candidate.Equals(end_vertex)) break;

				_determined_vertex_set.Add(cur_candidate);

				_improve_to_vertex(cur_candidate, is_source2sink);
			}
		}

		/**
	    * Update the distance from the source to the concerned vertex.
	    * @param vertex
	    */
		private void _improve_to_vertex(Vertex vertex, bool is_source2sink)
		{
			// 1. get the neighboring vertices 
			HashSet<Vertex> neighbor_vertex_list = is_source2sink ?
				_graph.get_adjacent_vertices(vertex) : _graph.get_precedent_vertices(vertex);

			// 2. update the distance passing on current vertex
			foreach (Vertex cur_adjacent_vertex in neighbor_vertex_list)
			{
				// 2.1 skip if visited before
				if (_determined_vertex_set.Contains(cur_adjacent_vertex)) continue;

				// 2.2 calculate the new distance
				double distance = _start_vertex_distance_index.ContainsKey(vertex) ?
					_start_vertex_distance_index[vertex] : Graph.DISCONNECTED;

				distance += is_source2sink ? _graph.get_edge_weight(vertex, cur_adjacent_vertex)
					: _graph.get_edge_weight(cur_adjacent_vertex, vertex);

				// 2.3 update the distance if necessary
				if (!_start_vertex_distance_index.ContainsKey(cur_adjacent_vertex)
				|| _start_vertex_distance_index[cur_adjacent_vertex] > distance)
				{
					_start_vertex_distance_index[cur_adjacent_vertex] = distance;

					_predecessor_index[cur_adjacent_vertex] = vertex;

					cur_adjacent_vertex.set_weight(distance);
					_vertex_candidate_queue.Add(cur_adjacent_vertex);
				}
			}
		}

		/**
	    * Note that, the source should not be as same as the sink! (we could extend 
	    * this later on)
	    *  
	    * @param source_vertex
	    * @param sink_vertex
	    * @return
	    */
		public Path get_shortest_path(Vertex source_vertex, Vertex sink_vertex)
		{
			determine_shortest_paths(source_vertex, sink_vertex, true);

			//
			List<Vertex> vertex_list = new List<Vertex>();
			double weight = _start_vertex_distance_index.ContainsKey(sink_vertex) ?
				_start_vertex_distance_index[sink_vertex] : Graph.DISCONNECTED;
			if (weight != Graph.DISCONNECTED)
			{
				Vertex cur_vertex = sink_vertex;
				do
				{
					vertex_list.Add(cur_vertex);
					cur_vertex = _predecessor_index[cur_vertex];
				} while (cur_vertex != null && cur_vertex != source_vertex);
				//
				vertex_list.Add(source_vertex);
				vertex_list.Reverse();
			}

			//
			return new Path(vertex_list, weight);
		}

		/// for updating the cost

		/**
	    * Calculate the distance from the target vertex to the input 
	    * vertex using forward star form. 
	    * (FLOWER)
	    * 
	    * @param vertex
	    */
		public Path update_cost_forward(Vertex vertex)
		{
			double cost = Graph.DISCONNECTED;

			// 1. get the set of successors of the input vertex
			HashSet<Vertex> adj_vertex_set = _graph.get_adjacent_vertices(vertex);

			// 2. make sure the input vertex exists in the index
			if (!_start_vertex_distance_index.ContainsKey(vertex))
			{
				_start_vertex_distance_index[vertex] = Graph.DISCONNECTED;
			}

			// 3. update the distance from the root to the input vertex if necessary
			foreach (Vertex cur_vertex in adj_vertex_set)
			{
				// 3.1 get the distance from the root to one successor of the input vertex
				double distance = _start_vertex_distance_index.ContainsKey(cur_vertex) ?
						_start_vertex_distance_index[cur_vertex] : Graph.DISCONNECTED;

				// 3.2 calculate the distance from the root to the input vertex
				distance += _graph.get_edge_weight(vertex, cur_vertex);
				//distance += ((VariableGraph)_graph).get_edge_weight_of_graph(vertex, cur_vertex);

				// 3.3 update the distance if necessary 
				double cost_of_vertex = _start_vertex_distance_index[vertex];
				if (cost_of_vertex > distance)
				{
					_start_vertex_distance_index[vertex] = distance;
					_predecessor_index[vertex] = cur_vertex;
					cost = distance;
				}
			}

			// 4. create the sub_path if exists
			Path sub_path = null;
			if (cost < Graph.DISCONNECTED)
			{
				sub_path = new Path();
				sub_path.set_weight(cost);
				List<Vertex> vertex_list = sub_path.get_vertices();
				vertex_list.Add(vertex);

				Vertex sel_vertex = _predecessor_index[vertex];
				while (_predecessor_index.ContainsKey(sel_vertex))
				{
					vertex_list.Add(sel_vertex);
					sel_vertex = _predecessor_index[sel_vertex];
				}
				vertex_list.Add(sel_vertex);
				/*
                while (sel_vertex != null)
                {
                    vertex_list.Add(sel_vertex);
                    sel_vertex = _predecessor_index[sel_vertex];
                }
                 */
			}

			return sub_path;
		}

		/**
	    * Correct costs of successors of the input vertex using backward star form.
	    * (FLOWER)
	    * 
	    * @param vertex
	    */
		public void correct_cost_backward(Vertex vertex)
		{
			// 1. initialize the list of vertex to be updated
			List<Vertex> vertex_list = new List<Vertex>();
			vertex_list.Add(vertex);

			// 2. update the cost of relevant precedents of the input vertex
			while (vertex_list.Count > 0)
			{
				Vertex cur_vertex = vertex_list[0];
				vertex_list.RemoveAt(0);
				double cost_of_cur_vertex = _start_vertex_distance_index[cur_vertex];

				HashSet<Vertex> pre_vertex_set = _graph.get_precedent_vertices(cur_vertex);
				foreach (Vertex pre_vertex in pre_vertex_set)
				{
					double cost_of_pre_vertex = _start_vertex_distance_index.ContainsKey(pre_vertex) ?
							_start_vertex_distance_index[pre_vertex] : Graph.DISCONNECTED;

					double fresh_cost = cost_of_cur_vertex + _graph.get_edge_weight(pre_vertex, cur_vertex);
					//double fresh_cost = cost_of_cur_vertex + ((VariableGraph)_graph).get_edge_weight_of_graph(pre_vertex, cur_vertex);
					if (cost_of_pre_vertex > fresh_cost)
					{
						_start_vertex_distance_index[pre_vertex] = fresh_cost;
						_predecessor_index[pre_vertex] = cur_vertex;
						vertex_list.Add(pre_vertex);
					}
				}
			}
		}
	}


	public class YenTopKShortestPathsAlg
	{
		private VariableGraph _graph = null;

		// intermediate variables
		private List<Path> _result_list;
		private Dictionary<Path, Vertex> _path_derivation_vertex_index;
		private QYPriorityQueue<Path> _path_candidates;

		//
		private Vertex _source_vertex = null;
		private Vertex _target_vertex = null;

		/**
         * Default constructor.
         * 
         * @param graph
         * @param k
         */
		public YenTopKShortestPathsAlg(Graph graph)
		{
			_result_list = new List<Path>();
			_path_derivation_vertex_index = new Dictionary<Path, Vertex>();
			_path_candidates = new QYPriorityQueue<Path>();

			if (graph == null)
			{
				throw new ArgumentException("A NULL graph object occurs!");
			}
			//
			_graph = new VariableGraph((Graph)graph);
			_source_vertex = null;
			_target_vertex = null;
			//
			_init();
		}

		/**
         * Constructor 2
         * 
         * @param graph
         * @param source_vt
         * @param target_vt
         */
		public YenTopKShortestPathsAlg(Graph graph,
				Vertex source_vt, Vertex target_vt)
		{
			_result_list = new List<Path>();
			_path_derivation_vertex_index = new Dictionary<Path, Vertex>();
			_path_candidates = new QYPriorityQueue<Path>();

			if (graph == null)
			{
				throw new ArgumentException("A NULL graph object occurs!");
			}
			//
			_graph = new VariableGraph((Graph)graph);
			_source_vertex = source_vt;
			_target_vertex = target_vt;
			//
			_init();
		}

		/**
         * Initiate members in the class. 
         */
		private void _init()
		{
			clear();
			// get the shortest path by default if both source and target exist
			if (_source_vertex != null && _target_vertex != null)
			{
				Path shortest_path = get_shortest_path(_source_vertex, _target_vertex);
				if (shortest_path.get_vertices().Count > 0)
				{
					_path_candidates.add(shortest_path);
					_path_derivation_vertex_index[shortest_path] = _source_vertex;
				}
			}
		}

		/**
         * Clear the variables of the class. 
         */
		public void clear()
		{
			_path_candidates.Clear();// new QYPriorityQueue<Path>();
			_path_derivation_vertex_index.Clear();
			_result_list.Clear();
		}

		/**
         * Obtain the shortest path connecting the source and the target, by using the
         * classical Dijkstra shortest path algorithm. 
         * 
         * @param source_vt
         * @param target_vt
         * @return
         */
		public Path get_shortest_path(Vertex source_vt, Vertex target_vt)
		{
			DijkstraShortestPathAlg dijkstra_alg = new DijkstraShortestPathAlg(_graph);
			return dijkstra_alg.get_shortest_path(source_vt, target_vt);
		}

		/**
         * Check if there exists a path, which is the shortest among all candidates.  
         * 
         * @return
         */
		public bool has_next()
		{
			return !_path_candidates.isEmpty();
		}

		/**
         * Get the shortest path among all that connecting source with targe. 
         * 
         * @return
         */
		public Path next()
		{
			//3.1 prepare for removing vertices and arcs
			Path cur_path = _path_candidates.poll();
			_result_list.Add(cur_path);

			Vertex cur_derivation = _path_derivation_vertex_index[cur_path];

			int count = _result_list.Count;

			//3.2 remove the vertices and arcs in the graph
			for (int i = 0; i < count - 1; ++i)
			{
				Path cur_result_path = _result_list.ElementAt(i);

				int cur_dev_vertex_id =
					cur_result_path.get_vertices().IndexOf(cur_derivation);

				Vertex cur_succ_vertex =
					cur_result_path.get_vertices().ElementAt(cur_dev_vertex_id + 1);

				_graph.remove_edge(new KeyValuePair<int, int>(
						cur_derivation.get_id(), cur_succ_vertex.get_id()));
			}

			int path_length = cur_path.get_vertices().Count;
			List<Vertex> cur_path_vertex_list = cur_path.get_vertices();
			for (int i = 0; i < path_length - 1; ++i)
			{
				_graph.remove_vertex(cur_path_vertex_list.ElementAt(i).get_id());
				_graph.remove_edge(new KeyValuePair<int, int>(
						cur_path_vertex_list.ElementAt(i).get_id(),
						cur_path_vertex_list.ElementAt(i + 1).get_id()));
			}

			//3.3 calculate the shortest tree rooted at target vertex in the graph
			DijkstraShortestPathAlg reverse_tree = new DijkstraShortestPathAlg((VariableGraph)_graph);
			reverse_tree.get_shortest_path_flower(_target_vertex);

			//3.4 recover the deleted vertices and update the cost and identify the new candidate results
			bool is_done = false;
			for (int i = path_length - 2; i >= 0 && !is_done; --i)
			{
				//3.4.1 get the vertex to be recovered
				Vertex cur_recover_vertex = cur_path_vertex_list.ElementAt(i);
				_graph.recover_removed_vertex(cur_recover_vertex.get_id());

				//3.4.2 check if we should stop continuing in the next iteration
				if (cur_recover_vertex.get_id() == cur_derivation.get_id())
				{
					is_done = true;
				}

				//3.4.3 calculate cost using forward star form
				Path sub_path = reverse_tree.update_cost_forward(cur_recover_vertex);

				//3.4.4 get one candidate result if possible
				if (sub_path != null)
				{
					//3.4.4.1 get the prefix from the concerned path
					double cost = 0;
					List<Vertex> pre_path_list = new List<Vertex>();
					reverse_tree.correct_cost_backward(cur_recover_vertex);

					for (int j = 0; j < path_length; ++j)
					{
						Vertex cur_vertex = cur_path_vertex_list.ElementAt(j);
						if (cur_vertex.get_id() == cur_recover_vertex.get_id())
						{
							j = path_length;
						}
						else
						{
							cost += _graph.get_edge_weight_of_graph(cur_path_vertex_list.ElementAt(j),
									cur_path_vertex_list.ElementAt(j + 1));
							pre_path_list.Add(cur_vertex);
						}
					}
					pre_path_list.AddRange(sub_path.get_vertices());

					//3.4.4.2 compose a candidate
					sub_path.set_weight(cost + sub_path.get_weight());
					sub_path.get_vertices().Clear();
					sub_path.get_vertices().AddRange(pre_path_list);

					//3.4.4.3 put it in the candidate pool if new
					if (!_path_derivation_vertex_index.ContainsKey(sub_path))
					{
						_path_candidates.add(sub_path);
						_path_derivation_vertex_index[sub_path] = cur_recover_vertex;
					}
				}

				//3.4.5 restore the edge
				Vertex succ_vertex = cur_path_vertex_list.ElementAt(i + 1);
				_graph.recover_removed_edge(new KeyValuePair<int, int>(
						cur_recover_vertex.get_id(), succ_vertex.get_id()));

				//3.4.6 update cost if necessary
				double cost_1 = _graph.get_edge_weight(cur_recover_vertex, succ_vertex)
					+ reverse_tree.get_start_vertex_distance_index()[succ_vertex];

				if (reverse_tree.get_start_vertex_distance_index()[cur_recover_vertex] > cost_1)
				{
					reverse_tree.get_start_vertex_distance_index()[cur_recover_vertex] = cost_1;
					reverse_tree.get_predecessor_index()[cur_recover_vertex] = succ_vertex;
					reverse_tree.correct_cost_backward(cur_recover_vertex);
				}
			}

			//3.5 restore everything
			_graph.recover_removed_edges();
			_graph.recover_removed_vertices();

			//
			return cur_path;
		}

		/**
         * Get the top-K shortest paths connecting the source and the target.  
         * This is a batch execution of top-K results.
         * 
         * @param source
         * @param sink
         * @param top_k
         * @return
         */
		public List<Path> get_shortest_paths(Vertex source_vertex,
				Vertex target_vertex, int top_k)
		{
			_source_vertex = source_vertex;
			_target_vertex = target_vertex;

			_init();
			int count = 0;
			while (has_next() && count < top_k)
			{
				next();
				++count;
			}

			return _result_list;
		}

		/**
         * Get the top-K shortest paths connecting the source and the target.  
         * The function of the following method is not recommended, please use get_shortest_paths(). 
         * 
         * @deprecated
         * @param source
         * @param sink
         * @param top_k
         * @return
         */
		public List<Path> get_shortest_paths1(Vertex source_vertex,
				Vertex target_vertex, int top_k)
		{
			DijkstraShortestPathAlg dijkstra_alg = new DijkstraShortestPathAlg(_graph);
			//1. achieve the shortest path in the graph from source to target
			Path shortest_path = dijkstra_alg.get_shortest_path(source_vertex, target_vertex);

			//2. initialize the variables
			clear();
			int count = 0;

			_path_candidates.add(shortest_path);

			_path_derivation_vertex_index[shortest_path] = source_vertex;

			//3. main loop of the algorithm
			while (!_path_candidates.isEmpty() && count < top_k)
			{
				//3.1 prepare for removing vertices and arcs
				Path cur_path = _path_candidates.poll();
				_result_list.Add(cur_path);

				Vertex cur_derivation = _path_derivation_vertex_index[cur_path];

				++count;

				//3.2 remove the vertices and arcs in the graph
				for (int i = 0; i < count - 1; ++i)
				{
					Path cur_result_path = _result_list.ElementAt(i);

					int cur_dev_vertex_id =
						cur_result_path.get_vertices().IndexOf(cur_derivation);

					Vertex cur_succ_vertex =
						cur_result_path.get_vertices().ElementAt(cur_dev_vertex_id + 1);

					_graph.remove_edge(new KeyValuePair<int, int>(
							cur_derivation.get_id(), cur_succ_vertex.get_id()));
				}

				int path_length = cur_path.get_vertices().Count;
				List<Vertex> cur_path_vertex_list = cur_path.get_vertices();
				for (int i = 0; i < path_length - 1; ++i)
				{
					_graph.remove_vertex(cur_path_vertex_list.ElementAt(i).get_id());
					_graph.remove_edge(new KeyValuePair<int, int>(
							cur_path_vertex_list.ElementAt(i).get_id(),
							cur_path_vertex_list.ElementAt(i + 1).get_id()));
				}

				//3.3 calculate the shortest tree rooted at target vertex in the graph
				DijkstraShortestPathAlg reverse_tree = new DijkstraShortestPathAlg(_graph);
				reverse_tree.get_shortest_path_flower(target_vertex);

				//3.4 recover the deleted vertices and update the cost and identify the new candidate results
				bool is_done = false;
				for (int i = path_length - 2; i >= 0 && !is_done; --i)
				{
					//3.4.1 get the vertex to be recovered
					Vertex cur_recover_vertex = cur_path_vertex_list.ElementAt(i);
					_graph.recover_removed_vertex(cur_recover_vertex.get_id());

					//3.4.2 check if we should stop continuing in the next iteration
					if (cur_recover_vertex.get_id() == cur_derivation.get_id())
					{
						is_done = true;
					}

					//3.4.3 calculate cost using forward star form
					Path sub_path = reverse_tree.update_cost_forward(cur_recover_vertex);

					//3.4.4 get one candidate result if possible
					if (sub_path != null)
					{
						//3.4.4.1 get the prefix from the concerned path
						double cost = 0;
						List<Vertex> pre_path_list = new List<Vertex>();
						reverse_tree.correct_cost_backward(cur_recover_vertex);

						for (int j = 0; j < path_length; ++j)
						{
							Vertex cur_vertex = cur_path_vertex_list.ElementAt(j);
							if (cur_vertex.get_id() == cur_recover_vertex.get_id())
							{
								j = path_length;
							}
							else
							{
								cost += _graph.get_edge_weight_of_graph(cur_path_vertex_list.ElementAt(j),
										cur_path_vertex_list.ElementAt(j + 1));
								pre_path_list.Add(cur_vertex);
							}
						}
						pre_path_list.AddRange(sub_path.get_vertices());

						//3.4.4.2 compose a candidate
						sub_path.set_weight(cost + sub_path.get_weight());
						sub_path.get_vertices().Clear();
						sub_path.get_vertices().AddRange(pre_path_list);

						//3.4.4.3 put it in the candidate pool if new
						if (!_path_derivation_vertex_index.ContainsKey(sub_path))
						{
							_path_candidates.add(sub_path);
							_path_derivation_vertex_index[sub_path] = cur_recover_vertex;
						}
					}

					//3.4.5 restore the edge
					Vertex succ_vertex = cur_path_vertex_list.ElementAt(i + 1);
					_graph.recover_removed_edge(new KeyValuePair<int, int>(
							cur_recover_vertex.get_id(), succ_vertex.get_id()));

					//3.4.6 update cost if necessary
					double cost_1 = _graph.get_edge_weight(cur_recover_vertex, succ_vertex)
						+ reverse_tree.get_start_vertex_distance_index()[succ_vertex];

					if (reverse_tree.get_start_vertex_distance_index()[cur_recover_vertex] > cost_1)
					{
						reverse_tree.get_start_vertex_distance_index()[cur_recover_vertex] = cost_1;
						reverse_tree.get_predecessor_index()[cur_recover_vertex] = succ_vertex;
						reverse_tree.correct_cost_backward(cur_recover_vertex);
					}
				}

				//3.5 restore everything
				_graph.recover_removed_edges();
				_graph.recover_removed_vertices();
			}

			return _result_list;
		}

		/**
         * Return the list of results generated on the whole.
         * (Note that some of them are duplicates)
         * @return
         */
		public List<Path> get_result_list()
		{
			return _result_list;
		}

		/**
         * The number of distinct candidates generated on the whole. 
         * @return
         */
		public int get_cadidate_size()
		{
			return _path_derivation_vertex_index.Count;
		}
	}
}
