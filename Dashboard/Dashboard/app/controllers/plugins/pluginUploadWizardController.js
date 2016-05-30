(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('pluginUploadWizardController',
        ['$scope', 'FileUploader', '$http', '$timeout', 'installationStepStates', 'installationSteps', 'communicationTypes','authService',

            pluginUploadWizardController]);

    function pluginUploadWizardController($scope, FileUploader, $http, $timeout,
        installationStepStates, installationSteps, communicationTypes, authService) {

        $scope.uploader = new FileUploader({
            url: 'api/plugins/upload',
            headers: {},
            withCredentials: true
        });

        $scope.roundPluginSize = function (size) {
            var number = new Number(size * 0.000001);
            return number.toFixed(2);
        }

        $scope.getCommunicationTypeLabel = function (index) {
            return communicationTypes[index];
        }

        $scope.goToNextStep = function () {
            var currentStep = $scope.installationSteps[$scope.stepNumber];
            if (currentStep.state === installationStepStates.completed) {
                $scope.stepNumber++;
                currentStep = $scope.installationSteps[$scope.stepNumber];
                currentStep.state = installationStepStates.current;
                return true;
            }
            return false;
        };

        $scope.currentStepPassed = function () {
            var currentStep = $scope.installationSteps[$scope.stepNumber];
            currentStep.state = installationStepStates.completed;
        };

        $scope.currentStepError = function () {
            var currentStep = $scope.installationSteps[$scope.stepNumber];
            currentStep.state = installationStepStates.error;
            $scope.stepContext.hasErrors = true;
        };

        $scope.validatePlugin = function () {

            if ($scope.goToNextStep()) {
                $http.post('api/plugins/validate/' + $scope.stepContext.fileId)
                .success(function (response) {
                    $scope.stepContext.validation.validationPassed = response.isValidated;
                    $scope.stepContext.validation.validationResults = response.pluginValidationResults;
                    $scope.currentStepPassed();
                    $scope.stepContext.validation.loading = false;
                })
                .error(function (err) {
                    $scope.stepContext.validation.validationPassed = false;
                    $scope.stepContext.validation.validationResults = err.pluginValidationResults;
                    $scope.currentStepError();
                    $scope.stepContext.validation.loading = false;
                });
            }
        }

        $scope.getInformation = function () {
            if ($scope.goToNextStep()) {
                $http.post('api/plugins/info/' + $scope.stepContext.fileId)
                   .success(function (response) {
                       $scope.stepContext.information.pluginInfo = response;
                       $scope.stepContext.information.loadedOk = true;
                       $scope.currentStepPassed();
                       $scope.stepContext.information.loading = false;
                       $timeout(function () {
                           SyntaxHighlighter.highlight();
                       }, 0);
                   })
                   .error(function (err) {
                       $scope.stepContext.information.loadedOk = false;
                       $scope.stepContext.information.pluginInfo = err;
                       $scope.stepContext.information.loading = false;
                       $scope.currentStepError();
                   });
            }
        }

        $scope.installPlugin = function () {
            if ($scope.goToNextStep()) {
                $http.post('api/plugins/install/' + $scope.stepContext.fileId)
                   .success(function (response) {
                       $scope.currentStepPassed();
                       $scope.stepContext.installation.installedOk = true;
                       $scope.stepContext.installation.loading = false;
                       $scope.goToNextStep();
                       $scope.currentStepPassed();
                   })
                   .error(function (err) {
                       $scope.currentStepError();
                       $scope.stepContext.installation.installedOk = false;
                       $scope.stepContext.installation.loading = false;
                       $scope.stepContext.installation.error = err;
                       console.error(err);
                   });
            }
        }

        $scope.initialize = function () {
            $scope.stepNumber = 0;
            $scope.installationSteps = angular.copy(installationSteps);
            var currentStep = $scope.installationSteps[$scope.stepNumber];
            currentStep.state = installationStepStates.current;
            $scope.uploader.clearQueue();
            $scope.stepContext = {
                upload: {
                    fileDropVisible: true,
                    fileUploadError: undefined
                },
                validation: {
                    validationResults: [],
                    validationPassed: undefined,
                    loading: true
                },
                information: {
                    pluginInfo: undefined,
                    loadedOk: false,
                    loading: true
                },
                installation: {
                    installedOk: false,
                    errors: undefined,
                    loading: true
                },

                fileId: "",
                validationResults: "",
                pluginInfo: "",
                hasErrors: false
            };
        };

        (function () {
            configureFileUpload($scope, $timeout, authService);
            $scope.initialize();
        })();
    }
})();

angular
    .module('angularAdmin').constant('installationStepStates', {
        current: {
            cssClass: 'fa fa-circle-o'
        },
        completed: {
            cssClass: 'fa fa-check-square'
        },
        error: {
            cssClass: 'fa fa-times-circle'
        }
    }).constant('installationSteps', [
        { name: 'upload', state: undefined },
        { name: 'validation', state: undefined },
        { name: 'information', state: undefined },
        { name: 'installation', state: undefined },
        { name: 'completed', state: undefined }
    ]);

function configureFileUpload($scope, $timeout, authService) {

    var zipFileRegEx = /.+\.zip/i;

    $scope.uploader.filters.push({
        name: 'fileCountFilter',
        fn: function (item, options) {
            return this.queue.length < 2;
        }
    });

    $scope.uploader.onAfterAddingFile = function (fileItem) {
        $scope.stepContext.upload.fileDropVisible = false;
    };

    $scope.removeItemFromUploadQueue = function (item) {
        $scope.stepContext.upload.fileDropVisible = true;
        item.remove();
    };

    $scope.uploader.filters.push({
        name: 'fileFormatFilter',
        fn: function (item, options) {
            return zipFileRegEx.test(item.name);
        }
    });

    $scope.uploader.onBeforeUploadItem = function (fileItem) {
        var bearerHeader = authService.getAuthorizationHeader();
        fileItem.headers = angular.extend({ Authorization: bearerHeader }, fileItem.headers || {});
    };

    $scope.uploader.onSuccessItem = function (fileItem, response, status, headers) {
        $scope.stepContext.fileId = response.fileId;
        $scope.currentStepPassed();
        $timeout($scope.validatePlugin, 2000);
    }

    $scope.uploader.onErrorItem = function (fileItem, response, status, headers) {
        $scope.stepContext.upload.fileUploadError = response.message;
        $scope.currentStepError();
    }
}

var pluginInfo = {
    "name": "TestApp",
    "pluginId": "3F2504E0-4F89-41D3-9A0C-0305E82C3302",
    "version": "1.1.1111.1115",
    "communicationType": 0,
    "startingProgram": "Dashboard.exe",
    "methodsCount": 2,
    "configurationJson": "{\r\n\t\"some_setting\" : \"Hey Mama\"\r\n}",
    "rawXml": "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<plugin>\r\n  <name>TestApp</name>\r\n  <pluginId>3F2504E0-4F89-41D3-9A0C-0305E82C3302</pluginId>\r\n    <version>1.1.1111.1115</version>\r\n  <communicationType>Plain</communicationType>\r\n  <startingProgram>Dashboard.exe</startingProgram>\r\n  <icon>icon.ico</icon>\r\n  <api>\r\n    <method name=\"GetData\">\r\n      <inputType>String</inputType>\r\n      <outputType>String</outputType>\r\n    </method>\r\n\t<method name=\"SOME\">\r\n      <inputType>String</inputType>\r\n      <outputType>String</outputType>\r\n    </method>\r\n  </api>\r\n</plugin>",
    "zipInfo": {
        "archiveSize": 183253,
        "filesCount": 7,
        "uncompressedSize": 334808,
        "issuerName": "PluginBasicZipInformationExtractor"
    }
};