﻿namespace LogSample.Model.Model
{

    public class ElasticModel<T> where T : class
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public int _version { get; set; }
        public int _seq_no { get; set; }
        public int _primary_term { get; set; }
        public bool found { get; set; }
        public T _source { get; set; }
    }



}
