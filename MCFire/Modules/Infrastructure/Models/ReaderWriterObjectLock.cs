using System;
using System.Threading;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;

namespace MCFire.Modules.Infrastructure.Models
{
    /// <summary>
    /// A wrapper on the ReaderWriterLockSlim that provides concurrent reading and exclusive write access to a resource.
    /// </summary>
    public class ReaderWriterObjectLock<T>
    {
        readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly T _resource;
        // TODO: internal
        public ReaderWriterObjectLock([CanBeNull]T resource)
        {
            _resource = resource;
        }

        public void Access(AccessMode accessMode, Action<T> interactFunc)
        {
            switch (accessMode)
            {
                case AccessMode.Read:
                    try
                    {
                        ReaderWriterLock.EnterUpgradeableReadLock();
                        interactFunc.Invoke(Resource);

                    }
                    finally
                    {
                        ReaderWriterLock.ExitUpgradeableReadLock();
                    }
                    break;
                case AccessMode.ReadWrite:
                    try
                    {
                        ReaderWriterLock.EnterWriteLock();
                        interactFunc.Invoke(Resource);
                    }
                    finally
                    {
                        ReaderWriterLock.ExitWriteLock();
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
                        ReaderWriterLock.EnterUpgradeableReadLock();
                        return _resource;
                    }
                    finally
                    {
                        ReaderWriterLock.ExitUpgradeableReadLock();
                    }
                case AccessMode.ReadWrite:
                    try
                    {
                        ReaderWriterLock.EnterWriteLock();
                        return _resource;
                    }
                    finally
                    {
                        ReaderWriterLock.ExitWriteLock();
                    }
                default:
                    throw new ArgumentOutOfRangeException("accessMode");
            }
        }

        /// <summary>
        /// Ends access to the resource.
        /// </summary>
        /// <param name="accessMode"></param>
        public void EndAccess(AccessMode accessMode)
        {
            switch (accessMode)
            {
                case AccessMode.Read:
                    ReaderWriterLock.ExitUpgradeableReadLock();
                    break;
                case AccessMode.ReadWrite:
                    ReaderWriterLock.ExitWriteLock();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("accessMode");
            }
        }

        /// <summary>
        /// Returns the internal lock
        /// </summary>
        public ReaderWriterLockSlim ReaderWriterLock
        {
            get { return _readerWriterLock; }
        }

        /// <summary>
        /// Returns the internal resource
        /// </summary>
        public T Resource
        {
            get { return _resource; }
        }
    }
}
