package main

import (
	"fmt"
	"strconv"
	"encoding/json"
	"strings"
	"regexp"

	"github.com/inklabsfoundation/inkchain/core/chaincode/shim"
	pb "github.com/inklabsfoundation/inkchain/protos/peer"
)

const (
	// invoke func name
	MarkAsSchool                = "markAsSchool"
	UnmarkAsSchool              = "unmarkAsSchool"
	SetTranscript               = "setTranscript"
	QueryTranscript             = "queryTranscript"
	OfferInsuranceOrder         = "offerInsuranceOrder"
	AcceptInsuranceOrder        = "acceptInsuranceOrder"
	ExerciseInsuranceOrder      = "exerciseInsuranceOrder"
	AbandonInsuranceOrder       = "abandonInsuranceOrder"
	EnforceInsuranceOrder       = "enforceInsuranceOrder"
	Debug                       = "debug"
)

const (
	// TRANSCRIPT_School_TranscriptId
	TranscriptPrefix            = "TRANSCRIPT_"
	// INSURANCE_InsuranceId
	InsuranceOrderPrefix        = "INSURANCE_"
	// SCHOOL_School
	SchoolPrefix                = "SCHOOL_"
	OneDayUnixTimeSpan          = 86400	
)

var GuidRegex *regexp.Regexp = regexp.MustCompile("^[0-9a-fA-F]{32}$")

// Demo chaincode for asset registering, querying and transferring
type DatabaseChaincode struct {
}

type DecryptedTranscript struct {
	Student		string	`json:"student"`
	Year		uint	`json:"year"`
	Term		uint	`json:"term"`
	Course		string  `json:"course"`
	Credit		uint	`json:"credit"`
	Grade		uint	`json:"grade"`
}

type InsuranceOrderCall struct {
	School		string	`json:"school"`
	Student		string	`json:"student"`		// long
	Year		uint64	`json:"year"`
	Term		uint64	`json:"term"`
	Credit		uint64	`json:"credit"`
	Writer		string	`json:"writer"`			// short
	Strike		uint64	`json:"strike"`			// grade < strike <=> should exercise
	Premium		uint64	`json:"premium"`		// amount paid to accept the order
	Payoff		uint64	`json:"payoff"`			// amount paid if exercised
	OfferExpiry	uint64	`json:"offer_expiry"`	// must be accepted before this UNIX timestamp
	Accepted	bool	`json:"accepted"`
	RightExpiry	uint64	`json:"right_expiry"`	// must be exercised before this UNIX timestamp
	Exercised	bool	`json:"exercised"`
	Forced		bool	`json:"forced"`
}

func main() {
	err := shim.Start(new(DatabaseChaincode))
	if err != nil {
		fmt.Printf("Error starting DatabaseChaincode: %s", err)
	}
}

func (t *DatabaseChaincode) Init(stub shim.ChaincodeStubInterface) pb.Response {
	fmt.Println("DatabaseChaincode is initializing.")
	args := stub.GetStringArgs()
    if args == nil || len(args) != 2 {
        return shim.Error("INVALID_ARGUMENTS: Deploy(bureauOfEducationId).")
    }
    stub.PutState("BureauOfEducationId", []byte(args[1]))
	return shim.Success([]byte("Initialization was successful."))
}

func (t *DatabaseChaincode) Invoke(stub shim.ChaincodeStubInterface) pb.Response {
	function, args := stub.GetFunctionAndParameters()

	switch function {
	case MarkAsSchool:
		// sender = bureau of education
		// args[0] = school address
		return t.markAsSchool(stub, args)

	case UnmarkAsSchool:
		// sender = bureau of education
		// args[0] = school address
		return t.unmarkAsSchool(stub, args)

	case SetTranscript:
		// sender is the school
		// args[0] = transcript id
		// args[1] = encrypted transcript
		return t.setTranscript(stub, args)

	case QueryTranscript:
		// args[0] = school id
		// args[1] = transcript id
		return t.queryTranscript(stub, args)

	case OfferInsuranceOrder:
		// args[0] = insurance id
		// args[1] = school id
		// args[2] = student id = long
		// args[3] = year
		// args[4] = term
		// args[5] = credit
		// sender = writer = short
		// args[6] = strike
		// args[7] = premium
		// args[8] = payoff
		// args[9] = offer_expiry
		// args[10] = right_expiry
		return t.offerInsuranceOrder(stub, args)

	case AcceptInsuranceOrder:
		// sender = student id
		// args[0] = insurance id
		return t.acceptInsuranceOrder(stub, args)

	case ExerciseInsuranceOrder:
		// sender = writer
		// args[0] = insurance id
		return t.exerciseInsuranceOrder(stub, args)

	case AbandonInsuranceOrder:
		// sender = student id
		// args[0] = insurance id
		return t.abandonInsuranceOrder(stub, args)

	case EnforceInsuranceOrder:
		// sender = student id
		// args[0] = insurance id
		// args[1] = transcript id
		// args[2] = secret key
		return t.enforceInsuranceOrder(stub, args)

	case Debug:
		return t.debug(stub, args)

	}

	return shim.Error("NO_INTERFACE: unknown function invoked.")
}

func (t *DatabaseChaincode) markAsSchool(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	bureauId := "i" + sender
	bureauEduId, err := stub.GetState("BureauOfEducationId")
	if err != nil || bureauEduId == nil {
		return shim.Error("UNEXPECTED: could not retrieve BureauOfEducationId.")
	}
	bureauOfEducationId := string(bureauId)
	if bureauId != bureauOfEducationId {
		return shim.Error("ACCESS_DENIED: MarkAsSchool can only be called by the bureau of education.")
	}
	if len(args) != 1 {
		return shim.Error("INVALID_ARGUMENTS: MarkAsSchool(schoolAddress).")
	}
	schoolId := args[0]
	if stub.PutState(SchoolPrefix + schoolId, []byte { 1 }) != nil {
		return shim.Error("E_UNEXPECTED: could not mark as school.")
	}
	return shim.Success([]byte("Marked as school."))
}

func (t *DatabaseChaincode) unmarkAsSchool(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	bureauId := "i" + sender
	bureauEduId, err := stub.GetState("BureauOfEducationId")
	if err != nil || bureauEduId == nil {
		return shim.Error("UNEXPECTED: could not retrieve BureauOfEducationId.")
	}
	bureauOfEducationId := string(bureauId)
	if bureauId != bureauOfEducationId {
		return shim.Error("ACCESS_DENIED: UnmarkAsSchool can only be called by the bureau of education.")
	}
	if len(args) != 1 {
		return shim.Error("INVALID_ARGUMENTS: UnmarkAsSchool(schoolAddress).")
	}
	schoolId := args[0]
	if stub.PutState(SchoolPrefix + schoolId, []byte { 0 }) != nil {
		return shim.Error("E_UNEXPECTED: could not unmark as school.")
	}
	return shim.Success([]byte("Unmarked as school."))
}

func (t *DatabaseChaincode) setTranscript(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	schoolId := "i" + sender
	isSchool, err := stub.GetState(SchoolPrefix + schoolId)
	if err != nil || isSchool[0] != 1 {
		return shim.Error("ACCESS_DENIED: SetTranscript can only be called by a school.")
	}
	if len(args) != 2 {
		return shim.Error("INVALID_ARGUMENTS: SetTranscript(transcriptId, transcriptPayload).")
	}
	if !GuidRegex.MatchString(args[0]) {
		return shim.Error("INVALID_ARGUMENTS: transcriptId must be N-formatted GUID.")
	}
	transcriptId := strings.ToLower(args[0])
	transcript := args[1]
	if stub.PutState(TranscriptPrefix + schoolId + "_" + transcriptId, []byte(transcript)) != nil {
		return shim.Error("E_UNEXPECTED: could not add or update transcript.")
	}
	return shim.Success([]byte("Added or updated transcript."))
}

func (t *DatabaseChaincode) queryTranscript(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	if len(args) != 2 {
		return shim.Error("INVALID_ARGUMENTS: QueryTranscript(schoolId, transcriptId).")
	}
	if !GuidRegex.MatchString(args[1]) {
		return shim.Error("INVALID_ARGUMENTS: transcriptId must be N-formatted GUID.")
	}
	schoolId := args[0]
	transcriptId := strings.ToLower(args[1])
	result, err := stub.GetState(TranscriptPrefix + schoolId + "_" + transcriptId)
	if err != nil || result == nil {
		return shim.Error("NOT_FOUND: the transcript is not found.")
	}
	return shim.Success(result)
}

func (t *DatabaseChaincode) offerInsuranceOrder(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	txTs, err := stub.GetTxTimestamp()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetTxTimestamp failed.")
	}
	unixNow := uint64(txTs.Seconds)
	if len(args) != 11 {
		return shim.Error("INVALID_ARGUMENTS: OfferInsuranceOrder(insuranceId, schoolId, studentId, year, term, credit, strike, premium, payoff, offer_expiry, right_expiry).")
	}
	if !GuidRegex.MatchString(args[0]) {
		return shim.Error("INVALID_ARGUMENTS: insuranceId must be N-formatted GUID.")
	}
	insuranceId := strings.ToLower(args[0])
	insurance := InsuranceOrderCall { Accepted: false, Exercised: false, Forced: false }
	insurance.School = args[1]
	insurance.Student = args[2]
	insurance.Year, err = strconv.ParseUint(args[3], 10, 32)
	if err != nil || insurance.Year < 2010 || insurance.Year > 2099 {
		return shim.Error("INVALID_ARGUMENTS: year must be between 2000 and 2099 (inclusive).")
	}
	insurance.Term, err = strconv.ParseUint(args[4], 10, 32)
	if err != nil || insurance.Term > 5 {
		return shim.Error("INVALID_ARGUMENTS: term must be 0, 1, 2 or 3.")
	}
	insurance.Credit, err = strconv.ParseUint(args[5], 10, 32)
	if err != nil {
		return shim.Error("INVALID_ARGUMENTS: credit must fit into uint32.")
	}
	insurance.Writer = "i" + sender
	insurance.Strike, err = strconv.ParseUint(args[6], 10, 32)
	if err != nil {
		return shim.Error("INVALID_ARGUMENTS: strike must fit into uint32.")
	}
	insurance.Premium, err = strconv.ParseUint(args[7], 10, 32)
	if err != nil {
		return shim.Error("INVALID_ARGUMENTS: premium must fit into uint32.")
	}
	insurance.Payoff, err = strconv.ParseUint(args[8], 10, 32)
	if err != nil || insurance.Payoff <= insurance.Premium {
		return shim.Error("INVALID_ARGUMENTS: payoff must be greater than premium.")
	}
	insurance.OfferExpiry, err = strconv.ParseUint(args[9], 10, 64)
	if err != nil || insurance.OfferExpiry < unixNow + 7 * OneDayUnixTimeSpan {
		return shim.Error("INVALID_ARGUMENTS: offer_expiry must be at least 7 days more from now.")
	}
	insurance.RightExpiry, err = strconv.ParseUint(args[10], 10, 64)
	if err != nil || insurance.RightExpiry < unixNow + 30 * OneDayUnixTimeSpan {
		return shim.Error("INVALID_ARGUMENTS: right_expiry must be at least 30 days more from now.")
	}
	return shim.Error("E_NOTIMPL: to be implemented " + insuranceId + ".")
}

func (t *DatabaseChaincode) acceptInsuranceOrder(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	studentId := "i" + sender
	txTs, err := stub.GetTxTimestamp()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetTxTimestamp failed.")
	}
	unixNow := uint64(txTs.Seconds)
	if unixNow == 0 {
		return shim.Error("E_TIME: " + sender + "?")
	}
	if len(args) != 1 {
		return shim.Error("INVALID_ARGUMENTS: AcceptInsuranceOrder(insuranceId).")
	}
	if !GuidRegex.MatchString(args[0]) {
		return shim.Error("INVALID_ARGUMENTS: insuranceId must be N-formatted GUID.")
	}
	insuranceId := strings.ToLower(args[0])
	return shim.Error("E_NOTIMPL: to be implemented, " + studentId + ", " + insuranceId + ".")
}

func (t *DatabaseChaincode) exerciseInsuranceOrder(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	writerId := "i" + sender
	txTs, err := stub.GetTxTimestamp()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetTxTimestamp failed.")
	}
	unixNow := uint64(txTs.Seconds)
	if unixNow == 0 {
		return shim.Error("E_TIME: " + sender + "?")
	}
	if len(args) != 1 {
		return shim.Error("INVALID_ARGUMENTS: ExerciseInsuranceOrder(insuranceId).")
	}
	if !GuidRegex.MatchString(args[0]) {
		return shim.Error("INVALID_ARGUMENTS: insuranceId must be N-formatted GUID.")
	}
	insuranceId := strings.ToLower(args[0])
	return shim.Error("E_NOTIMPL: to be implemented, " + writerId + ", " + insuranceId + ".")
}

func (t *DatabaseChaincode) abandonInsuranceOrder(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	studentId := "i" + sender
	txTs, err := stub.GetTxTimestamp()
	if err != nil {
		return shim.Error("GetTxTimestamp failed.")
	}
	unixNow := uint64(txTs.Seconds)
	if unixNow != 0 {
		return shim.Error("E_TIME: " + sender + ".")
	}
	if len(args) != 1 {
		return shim.Error("INVALID_ARGUMENTS: AbandonInsuranceOrder(insuranceId).")
	}
	if !GuidRegex.MatchString(args[0]) {
		return shim.Error("INVALID_ARGUMENTS: insuranceId must be N-formatted GUID.")
	}
	insuranceId := strings.ToLower(args[0])
	return shim.Error("E_NOTIMPL: to be implemented, " + studentId + ", " + insuranceId + ".")
}

func (t *DatabaseChaincode) enforceInsuranceOrder(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	sender, err := stub.GetSender()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetSender failed.")
	}
	sender = strings.ToLower(sender)
	studentId := "i" + sender
	txTs, err := stub.GetTxTimestamp()
	if err != nil {
		return shim.Error("E_UNEXPECTED: GetTxTimestamp failed.")
	}
	unixNow := uint64(txTs.Seconds)
	if unixNow != 0 {
		return shim.Error("E_TIME: " + sender + ".")
	}
	if len(args) != 3 {
		return shim.Error("INVALID_ARGUMENTS: EnforceInsuranceOrder(insuranceId, transcriptId, secretKey).")
	}
	if !GuidRegex.MatchString(args[0]) {
		return shim.Error("INVALID_ARGUMENTS: insuranceId must be N-formatted GUID.")
	}
	if !GuidRegex.MatchString(args[1]) {
		return shim.Error("INVALID_ARGUMENTS: transcriptId must be N-formatted GUID.")
	}
	insuranceId := strings.ToLower(args[0])
	transcriptId := strings.ToLower(args[1])
	secretKey := args[2]
	return shim.Error("E_NOTIMPL: to be implemented, " + studentId + ", " + insuranceId + ", " + transcriptId + ", " + secretKey + ".")
}

func (t *DatabaseChaincode) debug(stub shim.ChaincodeStubInterface, args []string) pb.Response {
	json.Marshal(nil)
	return shim.Error("ACCESS_DENIED: undebuggable.")
}
