using System;
using System.Collections.Generic;
using System.Linq;
using Readify.Useful.TeamFoundation.Common;
using Readify.Useful.TeamFoundation.Common.Notification;
using TfsDeployer.Configuration;
using TfsDeployer.Journal;
using TfsDeployer.TeamFoundation;

namespace TfsDeployer
{
    public class MappingProcessor : IMappingProcessor
    {

        private delegate void ExecuteDelegate(BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, Mapping mapping, IPostDeployAction postDeployAction, int deploymentId);

        private readonly IMappingEvaluator _mappingEvaluator;
        private readonly IDeploymentEventRecorder _deploymentEventRecorder;
        private readonly Func<IMappingExecutor> _executorFactory;

        public MappingProcessor(IMappingEvaluator mappingEvaluator, IDeploymentEventRecorder deploymentEventRecorder, Func<IMappingExecutor> executorFactory)
        {
            _mappingEvaluator = mappingEvaluator;
            _deploymentEventRecorder = deploymentEventRecorder;
            _executorFactory = executorFactory;
        }

        public void ProcessMappings(IEnumerable<Mapping> mappings, BuildStatusChangeEvent statusChanged, BuildDetail buildDetail, IPostDeployAction postDeployAction, int eventId)
        {
            var applicableMappings = from mapping in mappings
                                     where _mappingEvaluator.DoesMappingApply(mapping, statusChanged, buildDetail.Status.ToString())
                                     select mapping;

            foreach (var mapping in applicableMappings)
            {
                TraceHelper.TraceInformation(TraceSwitches.TfsDeployer,
                                             "Matching mapping found, executing, Computer:{0}, Script:{1}",
                                             mapping.Computer,
                                             mapping.Script);

                var deploymentId = _deploymentEventRecorder.RecordQueued(eventId, mapping.Script, mapping.Queue);

                var executor = _executorFactory();
                ExecuteDelegate executeDelegate = executor.Execute;
                executeDelegate.BeginInvoke(statusChanged, buildDetail, mapping, postDeployAction, deploymentId, ExecuteDelegateCallback, executeDelegate);
            }
        }

        private void ExecuteDelegateCallback(IAsyncResult asyncResult)
        {
            var executeDelegate = (ExecuteDelegate)asyncResult.AsyncState;
            var executor = (IMappingExecutor)executeDelegate.Target;
            try
            {
                executeDelegate.EndInvoke(asyncResult);
            } 
            finally
            {
                executor.Dispose();
            }
        }

    }
}