'use strict';
angular.module('homeApp')
.controller('gitChartsCtrl',
    [
    '$scope', '$http', '$interval', '$attrs','$filter',
    function ($scope, $http, $timeout, $attrs, $filter) {

        $scope.init($attrs);

        $scope.config = {
            apiLink: $scope.initialParams.dispatchLink,
                refreshRate: $scope.initialParams.config.refreshRate || 10000 * 6 * 3, // 3 minutes,
                branch: $scope.initialParams.config.branch,
                repoAddress: $scope.initialParams.config.repoAddress,
                daysToShow : $scope.initialParams.config.daysToShow
            };

            $scope.lastFetchedOn = undefined;
            $scope.fetchCommits = function () {
                var chartLabels = generateLabels($scope.config.daysToShow, $filter);
                var chartData = generateData($scope.config.daysToShow);
                $scope.chartData = {
                    labels: chartLabels,
                    series:['Commits per day'],
                    data: [chartData]
                };

                $scope.lastFetchedOn = Date.now();
                $timeout(function () {
                    $scope.fetchCommits();
                }, $scope.config.refreshRate);
            };

            $scope.fetchCommits();

            function generateLabels(daysToShow, $filter){
                var labels = [];
                var dateFilter = $filter('date');
                for (var i = daysToShow - 1 ; i >= 0; --i) {
                    var currentDate = new Date();
                    currentDate.setDate(currentDate.getDate() - i);
                    labels.push(dateFilter(currentDate,'MMM d'));
                }
                return labels;
            }

            function generateData(daysToShow){
                var data = [];
                for (var i = 0 ; i < daysToShow; i++) {
                    data.push(i);
                }
                return data;
            }
        }
        ]);
