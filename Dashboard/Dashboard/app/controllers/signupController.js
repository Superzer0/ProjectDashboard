'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService', 'notificationService',
    function ($scope, $location, $timeout, authService, notificationService) {

        var startTimer = function () {
            var timer = $timeout(function () {
                $timeout.cancel(timer);
                $location.path('/login');
            }, 2000);
        }

        $scope.savedSuccessfully = false;
        $scope.formBeingSubmitted = false;
        $scope.message = "";

        $scope.registration = {
            userName: "",
            password: "",
            confirmPassword: ""
        };

        $scope.submitNotPermitted = function () {
            return $scope.signUpForm.$invalid || $scope.passNotMatch() || $scope.formBeingSubmitted;
        }

        $scope.passNotMatch = function () {
            return $scope.registration.confirmPassword !== $scope.registration.password;
        }

        $scope.signUp = function () {

            if ($scope.submitNotPermitted()) return;

            $scope.formBeingSubmitted = true;
            $scope.formClass = "";
            authService.saveRegistration($scope.registration).then(function (response) {

                $scope.savedSuccessfully = true;
                $scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
                notificationService.addNotification("Registration", "User has been registered successfully. You can now log in.");
                startTimer();
            },
             function (response) {
                 if (response.status === 400) {
                     var errors = [];
                     for (var key in response.data.ModelState) {
                         if (response.data.ModelState.hasOwnProperty(key)) {
                             for (var i = 0; i < response.data.ModelState[key].length; i++) {
                                 errors.push(response.data.ModelState[key][i]);
                             }
                         }
                     }
                     $scope.message = "Failed to register user. Correct erros listed below: ";
                     $scope.errors = errors;

                 } else {
                     $scope.message = "Internal error, try again later";
                 }

                 $scope.formClass = "shake animated";
                 $scope.formBeingSubmitted = false;
             });
        };
    }]);