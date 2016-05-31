'use strict';
angular.module('homeApp')
    .controller('jenkinsController',
    [
        '$scope', '$http', '$interval','$attrs','$filter',
        function ($scope, $http, $timeout, $attrs, $filter) {

            var dateFilter = $filter('date');
            $scope.init($attrs);
            $scope.jobs = [];
            var statusJobs = {
                1: "build-status-icon-success",
                2: "build-status-icon-unstable",
                3: "build-status-icon-failed",
                4: "build-status-icon-disabled",
                5: "build-status-icon-aborted"
            };

            $scope.config = {
                apiLink: $scope.initialParams.dispatchLink,
                jobsToShow: $scope.initialParams.config.jobsToShow || 4,
                refreshRate: $scope.initialParams.config.refreshRate || 10000 * 6 * 3, // 3 minutes,
                jenkinsAddress: $scope.initialParams.config.jenkinsAddress
            };

            $scope.showContent = function(){
                return $scope.firstTimeLoading === false 
                    && angular.isArray($scope.jobs) && $scope.jobs.length > 0;                   
            };

            $scope.showError = function(){
                return $scope.firstTimeLoading === false 
                    && !angular.isArray($scope.jobs);
            };

            $scope.showEmptyState = function(){
                return $scope.firstTimeLoading === false 
                    && angular.isArray($scope.jobs) && $scope.jobs.length === 0;
            };

            $scope.showLoader = function(){
                return $scope.firstTimeLoading;
            };

            $scope.firstTimeLoading = true;
            $scope.lastFetchedOn = undefined;
            $scope.fetchJobs = function () {
        
                $http.post($scope.config.apiLink + '/get-jobs', "")
                .success(function(response){
                    $scope.jobs = response;
                    $scope.lastFetchedOn = Date.now();
                    $scope.firstTimeLoading = false;
                }).error(function(err){
                    $scope.firstTimeLoading = false;
                });

                $timeout(function () {
                    $scope.fetchJobs();
                }, $scope.config.refreshRate);
            };

            $scope.getStatusClass = function (statusCode) {
                return statusJobs[statusCode];
            }

            $scope.getFormattedDate = function(dateString){
                if(!dateString) return 'n/a';
                var date = new Date(dateString);
                return dateFilter(date,'medium');
            }

            $scope.fetchJobs();
        }
    ]);
