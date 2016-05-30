'use strict';
angular.module('homeApp')
    .controller('jenkinsController',
    [
        '$scope', '$http', '$interval','$attrs',
        function ($scope, $http, $timeout, $attrs) {

            $scope.init($attrs);
            $scope.jobs = [];
            var statusJobs = {
                1: "badge-icon-stable",
                2: "badge-icon-unstable",
                3: "badge-icon-fail"
            };

            $scope.config = {
                apiLink: $scope.initialParams.dispatchLink,
                jobsToShow: $scope.initialParams.config.jobsToShow || 4,
                refreshRate: $scope.initialParams.config.refreshRate || 10000 * 6 * 3, // 3 minutes,
                jenkinsAddress: $scope.initialParams.config.jenkinsAddress
            };
            console.log($scope.initialParams);
            $scope.lastFetchedOn = undefined;
            $scope.fetchJobs = function () {

                /*
                $http.post(config.apiLink + '/get-jobs', "")
                .success(function(response){
                    $scope.jobs = response;
                }).error(function(err){
                    console.error(err);
                });*/

                $scope.jobs = [
                {
                    Name: "some job 1",
                    IsInQueue: false,
                    Status: 0,
                    SuccessRate: 79,
                    LastBuildTime: 20
                },
                {
                    Name: "some job 2",
                    IsInQueue: false,
                    Status: 1,
                    SuccessRate: 20,
                    LastBuildTime: 40
                },
                {
                    Name: "some job 3",
                    IsInQueue: false,
                    Status: 3,
                    SuccessRate: 30,
                    LastBuildTime: 900
                },
                {
                    Name: "some job 4",
                    IsInQueue: false,
                    Status: 0,
                    SuccessRate: 79,
                    LastBuildTime: 20
                },
                {
                    Name: "some job 5",
                    IsInQueue: true,
                    Status: 1,
                    SuccessRate: 79,
                    LastBuildTime: 20
                },
                {
                    Name: "some job 6",
                    IsInQueue: false,
                    Status: 3,
                    SuccessRate: 20,
                    LastBuildTime: 10
                }

                ];

                $scope.lastFetchedOn = Date.now();

                $timeout(function () {
                    $scope.fetchJobs();
                }, $scope.config.refreshRate);
            };

            $scope.getStatusClass = function (statusCode) {
                return statusJobs[statusCode];
            }

            $scope.fetchJobs();
        }
    ]);
