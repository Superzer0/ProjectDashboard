'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService', 'notificationService', 'utilsService',
    function ($scope, $location, $timeout, authService, notificationService, utilsService) {

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
                     $scope.errors = utilsService.parseModelStateErrors(response.data);
                     if ($scope.errors.length > 0) {
                         $scope.message = "Failed to register user. Correct erros listed below: ";
                     } else {
                         $scope.message = "Failed to register user. Form contains errors.";
                     }
                 } else {
                     $scope.message = "Internal error, try again later";
                 }

                 $scope.formClass = "shake animated";
                 $scope.formBeingSubmitted = false;
             });
        };
    }]);