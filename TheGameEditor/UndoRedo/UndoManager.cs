using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TheGameEditor.UndoRedo
{
    public class UndoManager
    {
        public int Capacity { get; private set; }

        public bool CanUndo => history.Count > 0;
        public bool CanRedo => stash.Count > 0;


        private List<Command> history;
        private List<Command> stash;


        public UndoManager(int capacity)
        {
            Capacity = capacity;

            history = new List<Command>();
            stash = new List<Command>();
        }


        public void Undo()
        {
            if (IsEmpty(history))
            {
                return;
            }

            Command command = history[0];
            history.RemoveAt(0);
            stash.Insert(0, command);

            command.Undo();
        }

        public void Redo()
        {
            if (IsEmpty(stash))
            {
                return;
            }

            Command command = stash[0];
            stash.RemoveAt(0);
            history.Insert(0, command);

            command.Redo();
        }

        public void ReportCommand(Action action, Action undoAction)
        {
            var command = new Command(action, undoAction);
            history.Insert(0, command);

            stash.Clear();
        }


        private static bool IsEmpty<T>(IList<T> stack)
        {
            return stack.Count == 0;
        }


        private class Command
        {
            public Action Redo { get; private set; }
            public Action Undo { get; private set; }

            public Command(Action redo, Action undo)
            {
                Redo = redo;
                Undo = undo;
            }
        }
    }
}
