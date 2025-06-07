namespace WallyInterpreter.Components.Interpreter.Automaton
{
    public static class AutomatonOperatorsAndTools<T>
    {
        /// <summary>
        /// This methotd create the automaton to recognize de sequence
        /// </summary>
        /// <param name="sequence"> sequence to recognize </param>
        /// <param name="id"> for the name of automaton</param>
        /// <returns>automaton</returns>
        public static IAutomaton<T> SequenceAutomaton(T[] sequence, string id)
        {
            if (sequence.Length == 0) throw new Exception("Empty Sequence");
            IState<T> start = new State<T>(id, false, false);
            List<IState<T>> states = new List<IState<T>>();
            for (int i = 0; i < sequence.Length; i++) { 
                states.Add(new State<T>(id+i.ToString(), i == sequence.Length-1, false));
            }
            for(int i = 0; i < sequence.Length-1; i++)
            {
                states[i].AddTransition(sequence[i], states[i + 1]);
            }
            return new Automaton<T>(start, states,sequence);
        }
        /// <summary>
        /// Automaton U Automaton operation
        /// </summary>
        /// <param name="aut1">First Automaton</param>
        /// <param name="aut2">Second Automaton</param>
        /// <returns>Automaton Union</returns>
        public static IAutomaton<T> Union(IAutomaton<T> aut1, IAutomaton<T> aut2)
        {
            IState<T> start = new State<T>(aut1.Start().ID() + "_" + aut2.Start().ID()+"_Union",false,false);
            List<IState<T>> states = new List<IState<T>>() { start};
            var states1 = CopyStates(aut1);
            var states2 = CopyStates(aut2);
            var alphabet = aut1.Alphabet().ToList();
            //unir ambos alfabetos
            foreach (var symbol in aut2.Alphabet()) {
                if (!alphabet.Contains(symbol)) { 
                    alphabet.Add(symbol);
                }
            }
            //Anadir todos los estados del automata 1
            for (var i = 0; i < states1.Length; i++) { 
                states.Add(states1[i]);
            }
            //Anadir todos los estados del automata 2
            for (var i = 0; i < states2.Length; i++)
            {
                states.Add(states2[i]);
            }
            //Anado una epsilon transicion desde el estado inicial a los anteriores estados iniciales
            start.AddEpsilon(aut1.Start());
            start.AddEpsilon(aut2.Start());
            return new Automaton<T>(start, states, alphabet.ToArray()).ToDeterministic();
        }
        /// <summary>
        /// Concatenation of two automatons
        /// </summary>
        /// <param name="aut1">First automaton</param>
        /// <param name="aut2">Second automaton</param>
        /// <returns>Automaton Concated</returns>
        public static IAutomaton<T> Concat(IAutomaton<T> aut1, IAutomaton<T> aut2)
        {
            List<IState<T>> states = new List<IState<T>>();
            var states1 = CopyStates(aut1);
            var states2 = CopyStates(aut2);
            var alphabet = aut1.Alphabet();
            //Anadir el alfabeto
            foreach (var symbol in aut2.Alphabet())
            {
                if (!alphabet.Contains(symbol))
                {
                    alphabet.ToList().Add(symbol);
                }
            }
            //Anadir los estados del primer automata
            for (var i = 0; i < states1.Length; i++)
            {
                states.Add(states1[i]);
            }

            //Anadir los estados del segundo automata
            for (var i = 0; i < states2.Length; i++)
            {
                states.Add(states2[i]);
            }
            //A todos los estados finales del primer automata agregale una epsilon transition al inicial del segundo automata
            foreach(var state in aut1.Finals())
            {
                foreach(var st in states)
                {
                    if (st.ID() == state.ID())
                    {
                        st.AddEpsilon(aut2.Start());
                    }
                }    
            }
            return new Automaton<T>(aut1.Start(), states, alphabet).ToDeterministic();
        }
        public static IAutomaton<T> CopyAutomaton(IAutomaton<T> automaton)
        {
            var states = CopyStates(automaton);
            return new Automaton<T>(automaton.Start(), states.ToList(), automaton.Alphabet());
        }
        /// <summary>
        /// Create a copy the states of one automaton
        /// </summary>
        /// <param name="automaton">The automaton</param>
        /// <returns>The States of automaton</returns>
        public static IState<T>[] CopyStates(IAutomaton<T> automaton)
        {
            List<IState<T>> states = new List<IState<T>> ();
            Dictionary<string, Dictionary<T,string>> transitions = new Dictionary<string, Dictionary<T, string>> ();
            Dictionary<string, List<string>> epsilons = new Dictionary<string, List<string>> ();
            foreach(var state in automaton.States())
            {
                states.Add(new State<T>(state.ID(),state.IsAccepting(),state.IsFault()));
                transitions[state.ID()] = new Dictionary<T, string>();
                foreach(var ep in state.Epsilons())
                {
                    if(!epsilons.ContainsKey(ep.ID()))
                    {
                        epsilons[state.ID()] = new List<string> ();
                    }
                    epsilons[state.ID()].Add(ep.ID());
                }
                foreach (var symbol in automaton.Alphabet())
                {
                    if (!state.HasTransition(symbol))
                    {
                        continue;
                    }
                    transitions[state.ID()][symbol] = state.Next(symbol).ID();
                }
            }
            for(int i = 0; i < states.Count; i++)
            {
                if(epsilons.ContainsKey(states[i].ID()))
                {
                    foreach(var id in  epsilons[states[i].ID()])
                    {
                        foreach(var st in states)
                        {
                            if(st.ID() == id)
                                states[i].AddEpsilon(st);
                        }
                    }
                }
                foreach(var pair in transitions[states[i].ID()])
                {
                    foreach(var s in states)
                        if(s.ID() == pair.Value)
                            states[i].AddTransition(pair.Key, s);
                }
            }
            return states.ToArray();
        }
        /// <summary>
        /// Throw True if First Array of States its equal to the Second Array of states
        /// </summary>
        /// <param name="states1"> Array 1</param>
        /// <param name="states2"> Array 2</param>
        /// <returns>True: Array1 == Array2</returns>
        public static bool SetStatesEqual(IState<T>[] states1,IState<T>[] states2)
        {
            if(states1.Length != states2.Length)
            {
                return false;
            }
            foreach (var state1 in states1) {
                bool exist = false;
                foreach (var state2 in states2)
                {
                    if(state1.ID() == state2.ID())
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist) { 
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Verify if one Array the states if inside of a Matrix the States
        /// </summary>
        /// <param name="set_set_states"> Matrix the states </param>
        /// <param name="set_states">Array the states</param>
        /// <returns>True: Array pertenece Matrix</returns>
        public static bool SetStatesIsInSetofSetStates(IState<T>[][] set_set_states,IState<T>[] set_states)
        {
            foreach (var set_states_base in set_set_states) {
                if (SetStatesEqual(set_states_base, set_states))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Return the index in one state inside of array
        /// </summary>
        /// <param name="states">Array of States</param>
        /// <param name="state">State to search index</param>
        /// <returns>Intiger index</returns>
        public static bool Match(T[] s, IAutomaton<T> automaton)
        {
            foreach(var c in s)
            {
                try
                {
                    automaton.Walk(c);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return automaton.CurrentState().IsAccepting();
            
        }
    }
}
