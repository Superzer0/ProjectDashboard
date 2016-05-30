'use strict';
angular.module('homeApp')
    .controller('gitLatestCommitsCtrl',
    [
        '$scope', '$http', '$interval', '$attrs',
        function ($scope, $http, $timeout, $attrs) {

            $scope.init($attrs);
            $scope.commits = [];

            $scope.config = {
                apiLink: $scope.initialParams.dispatchLink,
                commitsToShow: $scope.initialParams.config.commitsToShow || 5,
                refreshRate: $scope.initialParams.config.refreshRate || 10000 * 6 * 3, // 3 minutes,
                branch: $scope.initialParams.config.branch,
                repoAdress: $scope.initialParams.config.repoAdress
            };

            $scope.lastFetchedOn = undefined;
 console.log($scope.initialParams);
            $scope.fetchCommits = function () {

                /*
                $http.post(apiLink + '/get-commits', "")
                .success(function(response){
                    $scope.commits = response;
                }).error(function(err){
                    console.error(err);
                });*/

                $scope.commits = [
	                {
	                    CommiterName: "Zbyszek Burakowski",
	                    Message: "RAC2016.1 EX Some very long commit",
	                    Date: Date.now(),
	                    Changes: 1,
	                },

	                {
	                    CommiterName: "Beata Biernacka",
	                    Message: "RAC2016.1 EX Importante",
	                    Date: Date.now() - 5000,
	                    Changes: 10,
	                },

	                {
	                    CommiterName: "Jakub Kawa",
	                    Message: "RAC2016.1 EX minor changes",
	                    Date: Date.now() - 2000,
	                    Changes: 30,
	                },

	                {
	                    CommiterName: "Katarzyna Czekaj",
	                    Message: "RAC2016.1 EX FIX",
	                    Date: Date.now() - 23423,
	                    Changes: 2,
	                }
	               
                ];

                $scope.lastFetchedOn = Date.now();

                $timeout(function () {
                    $scope.fetchCommits();
                }, $scope.config.refreshRate);
            };

            $scope.fetchCommits();
        }
    ]);
