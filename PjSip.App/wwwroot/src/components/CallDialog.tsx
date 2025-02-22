import { createSignal, Component } from 'solid-js';

interface CallDialogProps {
  accountId: string;
  onCall: (destination: string) => void;
  onClose: () => void;
}

export const CallDialog: Component<CallDialogProps> = (props) => {
  const [destination, setDestination] = createSignal('sip:1001@localhost');

  const handleSubmit = (e: Event) => {
    e.preventDefault();
    props.onCall(destination());
    props.onClose();
  };

  return (
    <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
      <div class="bg-gray-800 p-6 rounded-lg w-96">
        <h3 class="text-xl font-bold mb-4">Make Call</h3>
        <form onSubmit={handleSubmit} class="space-y-4">
          <input
            type="text"
            placeholder="sip:user@domain.com"
            class="w-full bg-gray-700 rounded-lg p-2"
            value={destination()}
            onInput={(e) => setDestination(e.currentTarget.value)}
            required
          />
          <div class="flex justify-end gap-2">
            <button
              type="button"
              class="px-4 py-2 bg-gray-700 rounded-lg"
              onClick={props.onClose}
            >
              Cancel
            </button>
            <button type="submit" class="px-4 py-2 bg-blue-600 rounded-lg">
              Call
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};