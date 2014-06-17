using System;
using System.Threading;
using JetBrains.Annotations;

namespace MCFire.Common.Infrastructure.Models
{

    namespace MCFire.Modules.Infrastructure.Models
    {
        /// <summary>
        /// A wrapper on the ReaderWriterLockSlim that provides concurrent reading and exclusive write access to a resource.
        /// </summary>
        public class ReaderWriterObjectLock<T>
        {
            private readonly T _resource;
            private ReaderWriterLockSlim _readerWriterLock;
            // TODO: internal
            public ReaderWriterObjectLock([CanBeNull]T resource)
            {
                _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
                _resource = resource;
            }

            public void Access(AccessMode accessMode, Action<T> interactFunc)
            {
                switch (accessMode)
                {
                    case AccessMode.Read:
                        try
                        {
                            _readerWriterLock.EnterUpgradeableReadLock();
                            interactFunc.Invoke(Resource);
                        }
                        finally
                        {
                            _readerWriterLock.ExitUpgradeableReadLock();
                        }
                        break;
                    case AccessMode.ReadWrite:
                        try
                        {
                            _readerWriterLock.EnterWriteLock();
                            interactFunc.Invoke(Resource);
                        }
                        finally
                        {
                            _readerWriterLock.ExitWriteLock();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("accessMode");
                }
            }

            /// <summary>
            /// Access the resource, EndAccess must be called when done.
            /// </summary>
            public T Access(AccessMode accessMode)
            {
                switch (accessMode)
                {
                    case AccessMode.Read:
                        try
                        {
                            _readerWriterLock.EnterUpgradeableReadLock();
                            return _resource;
                        }
                        finally
                        {
                            _readerWriterLock.ExitUpgradeableReadLock();
                        }
                    case AccessMode.ReadWrite:
                        try
                        {
                            _readerWriterLock.EnterWriteLock();
                            return _resource;
                        }
                        finally
                        {
                            _readerWriterLock.ExitWriteLock();
                        }
                    default:
                        throw new ArgumentOutOfRangeException("accessMode");
                }
            }

            /// <summary>
            /// Returns the internal resource
            /// </summary>
            public T Resource
            {
                get { return _resource; }
            }
        }

        public enum AccessMode
        {
            Read,
            ReadWrite
        }
    }
}