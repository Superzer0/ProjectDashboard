'use strict';
angular.module('homeApp')
.controller('gitChartsCtrl',
    [
    '$scope', '$http', '$interval', '$attrs','$filter','githubApiStatsActivity','githubApiStatsFrequency',
    function ($scope, $http, $timeout, $attrs, $filter, githubApiStatsActivity, githubApiStatsFrequency) {

        $scope.init($attrs);

        $scope.config = {
                apiLink: $scope.initialParams.dispatchLink,
                weeksToShow: $scope.initialParams.config.weeksToShow || 5,
                fromLastDays: $scope.initialParams.config.fromLastDays || 20,
                refreshRate: $scope.initialParams.config.refreshRate || 10000 * 6 * 3, // 3 minutes,
                repoOwner: $scope.initialParams.config.repoOwner || "twbs",
                repoName: $scope.initialParams.config.repoName || "bootstrap"
            };

            $scope.showContent = function(){
                return $scope.firstTimeLoading === false 
                    && angular.isDefined($scope.commitActivity) && angular.isDefined($scope.commitFrequency)
                        && ($scope.commitActivity.labels.length > 0 || $scope.commitFrequency.labels.length > 0);
            };

            $scope.showError = function(){
                return $scope.firstTimeLoading === false &&
                    (!angular.isDefined($scope.commitActivity) || !angular.isDefined($scope.commitFrequency));
            };

            $scope.showEmptyState = function(){
                return $scope.firstTimeLoading === false 
                    && angular.isDefined($scope.commitActivity) && angular.isDefined($scope.commitFrequency)
                        && ($scope.commitActivity.labels.length === 0 && $scope.commitFrequency.labels.length === 0);
            };

            $scope.showLoader = function(){
                return $scope.firstTimeLoading;
            };

            $scope.fetchCommits = function () {               
                
                 var dateFilter = $filter('date');

                 githubApiStatsActivity.query({
                    owner: $scope.config.repoOwner,
                    repo: $scope.config.repoName
                }, function(response){
                    $scope.commitActivity = parseResultsActivity(response, $scope.config.weeksToShow, dateFilter);
                    $scope.firstTimeLoading = false;
                    $scope.lastFetchedOn = Date.now();
                }, function(){
                    $scope.firstTimeLoading = false;
                    $scope.commitActivity = undefined;
                });

                 githubApiStatsFrequency.query({
                    owner: $scope.config.repoOwner,
                    repo: $scope.config.repoName
                }, function(response){
                    $scope.commitFrequency = parseResultsFrequency(response, $scope.config.weeksToShow, dateFilter);
                    $scope.firstTimeLoading = false;
                    $scope.lastFetchedOn = Date.now();
                }, function(){
                    $scope.firstTimeLoading = false;
                    $scope.commitFrequency = undefined;
                });


                $timeout(function () {
                    $scope.fetchCommits();
                 }, $scope.config.refreshRate);
            };
     
            function generateLabels(daysToShow, $filter){
                var labels = [];
                var dateFilter = $filter('date');
                for (var i = daysToShow - 1 ; i >= 0; --i) {
                    var currentDate = new Date();
                    currentDate.setDate(currentDate.getDate() - (i*7));
                    labels.push(dateFilter(currentDate,'MMM d'));
                }
                return labels;
            }

            function parseResultsActivity(response, weeksToShow, dateFilter){
                var commitActivity = {
                    labels: [],
                    series:['Commits per week'],
                    data: [[]]
                };

                if(!angular.isArray(response)){
                    return commitActivity;
                }

                response.sort(function(a,b){
                    if(a.week < b.week){
                        return 1;
                    }else if(a.week > b.week){
                        return -1;
                    }else{
                        return 0;
                    }
                });

                var dataToPresent = response.slice(0, weeksToShow).reverse();

                dataToPresent.forEach(function(entry){
                    commitActivity.labels.push(dateFilter(new Date(entry.week  * 1000), 'mediumDate'));
                    commitActivity.data[0].push(entry.total);
                });

                return commitActivity;
            }

            function parseResultsFrequency(response, weeksToShow, dateFilter){
                var codeFrequency = {
                    labels: [],
                    series:['Additions', 'Deletions'],
                    data: [[],[]]
                };
                
                if(!angular.isDefined(response)){
                    return codeFrequency;
                }

                 response.sort(function(a,b){
                    if(a[0] < b[0]){
                        return 1;
                    }else if(a[0] > b[0]){
                        return -1;
                    }else{
                        return 0;
                    }
                });

                var dataToPresent = response.slice(0, weeksToShow).reverse();

                 dataToPresent.forEach(function(entry){
                    codeFrequency.labels.push(dateFilter(new Date(entry[0] * 1000),'mediumDate'));
                    codeFrequency.data[0].push(entry[1]); // additions
                    codeFrequency.data[1].push(entry[2]); // deletions
                });

                return codeFrequency;
            }

            // inital values
            $scope.firstTimeLoading = true;
            $scope.lastFetchedOn = undefined;;

            $scope.commitActivity = {
                    labels: [],
                    series:['Commits per week'],
                    data: [[]]
            };

            $scope.commitFrequency = {
                    labels: [],
                    series:['Additions', 'Deletions'],
                    data: [[],[]]
            };

            $scope.fetchCommits();
        }
        ]);

angular.module('homeApp').factory('githubApiStatsActivity',['$resource', function($resource){
    return $resource('https://api.github.com/repos/:owner/:repo/stats/commit_activity'
    );
}]);

angular.module('homeApp').factory('githubApiStatsFrequency',['$resource', function($resource){
    return $resource('https://api.github.com/repos/:owner/:repo/stats/code_frequency'
    );
}]);