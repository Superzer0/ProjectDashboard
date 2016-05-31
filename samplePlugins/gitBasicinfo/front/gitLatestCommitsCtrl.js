'use strict';
angular.module('homeApp')
    .controller('gitLatestCommitsCtrl',
    [
        '$scope', '$http', '$interval', '$attrs','githubApi',
        function ($scope, $http, $timeout, $attrs, githubApi) {

            $scope.init($attrs);

            $scope.config = {
                apiLink: $scope.initialParams.dispatchLink,
                commitsToShow: $scope.initialParams.config.commitsToShow || 5,
                fromLastDays: $scope.initialParams.config.fromLastDays || 20,
                refreshRate: $scope.initialParams.config.refreshRate || 10000 * 6 * 3, // 3 minutes,
                repoOwner: $scope.initialParams.config.repoOwner || "twbs",
                repoName: $scope.initialParams.config.repoName || "bootstrap"
            };


            $scope.lastFetchedOn = undefined;
            $scope.firstTimeLoading = true;
            $scope.commits = [];

            // $scope.commits states:
            // $scope.commits ==  $scope.firstTimeLoading == true => display loading
            // $scope.commits ==  $scope.firstTimeLoading == false && $scope.commits = undefined => display error empty state
            // $scope.commits ==  $scope.firstTimeLoading == false && $scope.commits = [] => display empty state
            // $scope.commits ==  $scope.firstTimeLoading == false && $scope.commits = sth => display list

            $scope.showContent = function(){
                return $scope.firstTimeLoading == false && angular.isArray($scope.commits) && $scope.commits.length > 0;
            };

            $scope.showError = function(){
                return $scope.firstTimeLoading == false && !angular.isArray($scope.commits);
            };

            $scope.showEmptyState = function(){
                return $scope.firstTimeLoading == false && angular.isArray($scope.commits) && $scope.commits.length===0;
            };

            $scope.showLoader = function(){
                return $scope.firstTimeLoading;
            };

            $scope.fetchCommits = function () {

                var fetchSince = new Date();
                fetchSince.setDate(fetchSince.getDate() - $scope.config.fromLastDays);

                githubApi.query({
                    owner: $scope.config.repoOwner,
                    repo: $scope.config.repoName,
                    since: fetchSince.toISOString()
                }, function(response){
                    $scope.commits = parseResults(response);
                    $scope.firstTimeLoading = false;
                }, function(){
                    $scope.firstTimeLoading = false;
                    $scope.commits = undefined;
                });
               
                $scope.lastFetchedOn = Date.now();

                $timeout(function () {
                    $scope.fetchCommits();
                }, $scope.config.refreshRate);
            };

            $scope.fetchCommits();


            function parseResults(response){
                var commits = [];
                if(!response || !angular.isArray(response)) return commits;
                
                response.forEach(function(entry){

                var avatarUrl = "https://avatars.githubusercontent.com/u/2341234?v=3";
                    if(entry.author && entry.author.avatar_url){
                        avatarUrl = entry.author.avatar_url;
                    }

                    var commitEntry = {
                        name: entry.commit.author.name,
                        message: entry.commit.message,
                        date: new Date(entry.commit.author.date),
                        avatarUrl: avatarUrl
                    };

                    commits.push(commitEntry);
                });

                commits.sort(function(a,b){
                    if(a.date < b.date){
                        return 1;
                    }else if(a.date > b.date){
                        return -1;
                    }else{
                        return 0;
                    }
                });

                return commits;
            }

        }
    ]);

angular.module('homeApp').factory('githubApi',['$resource', function($resource){
    return $resource('https://api.github.com/repos/:owner/:repo/commits'
    );
}]);