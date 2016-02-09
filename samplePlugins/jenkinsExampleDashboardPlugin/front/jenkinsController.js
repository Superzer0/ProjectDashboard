angular.module('homeApp').controller('jenkinsController', ['$scope','$http', 
	function($scope,$http){

		$scope.jobs = []

		$scope.fetchJobs = function(){
			var apiLink = $('#main-div').attr('api-link');
			$http.post(apiLink + '/get-jobs', "")
			.success(function(response){
				$scope.jobs = response;
			}).error(function(err){
				console.error(err);
			});
		};

		$scope.fetchJobs();

	}])